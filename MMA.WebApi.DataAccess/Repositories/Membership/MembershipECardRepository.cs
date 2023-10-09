using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Enums;
using MMA.WebApi.Shared.Interfaces.Membership;
using MMA.WebApi.Shared.Models.Membership;
using MMA.WebApi.Shared.Models.ServiceNowModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.Membership
{
    public class MembershipECardRepository : IMembershipECardRepository
    {
        private readonly Func<MMADbContext> _contexFactory;

        public MembershipECardRepository(Func<MMADbContext> contexFactory)
        {
            _contexFactory = contexFactory;
        }

        public async Task<IEnumerable<MembershipModel>> GetAllMembershipsModel()
        {
            return await GetAllMemberships().Select(projectToMembershipModel).ToListAsync();
        }

        public async Task<IEnumerable<MembershipEcardModel>> GetMemberCard(string userId)
        {
            var context = _contexFactory();

            var cards = await context.MembershipECards
                .Where(x => x.OwnerId == userId && x.isMember)
                .Include(x => x.Membership)
                .Include(x => x.Membership.PictureData)
                .Select(x => MembershipModel(x))
                .ToListAsync();

            return cards;
        }

        private IQueryable<Models.Membership> GetAllMemberships()
        {
            var context = _contexFactory();
            var temp = context.Memberships.Include(x => x.PictureData);
            var listemp = temp.ToList();
            return temp;
        }

        public IEnumerable<MembershipModel> GetMembershipsForOffer(int offerId)
        {
            var context = _contexFactory();
            var memberships = context.OffersMemberships
                .Where(x => x.OfferId == offerId)
                .Include(x => x.Membership)
                .Select(x => x.Membership);
            return memberships.Select(projectToMembershipModel).ToHashSet();
        }

        public IEnumerable<MembershipModel> GetMembershipsForUser(string userId, bool isBuyer)
        {
            var context = _contexFactory();
            if (isBuyer)
            {
                var memberships = context.MembershipECards
                    .Where(x => x.OwnerId == userId && !x.isMember)
                    .Select(x => x.Membership);
                return CreateHashSet(memberships.Select(projectToMembershipModel).ToHashSet());
            }
            return CreateHashSet(context.Memberships.Select(projectToMembershipModel).ToHashSet());
        }

        public async Task<Dictionary<string, string>> GetMembershipTypes()
        {
            var context = _contexFactory();
            return context.Memberships.ToDictionary(x => x.Id.ToString(), x => x.NameEng);
        }

        public async Task<IEnumerable<MembershipEcardModel>> GetOwnerCards(string userId)
        {
            var context = _contexFactory();
            List<MembershipEcardModel> cards = new List<MembershipEcardModel>();
            cards.Add(await CreateOffersECard(userId));

            cards.AddRange(
                await context.MembershipECards
                    .Where(
                        x =>
                            (x.OwnerId == userId && !x.isMember)
                            || (x.MemberId == userId && x.isMember)
                    )
                    .Include(x => x.Membership)
                    .Include(x => x.Membership.PictureData)
                    .Select(x => MembershipModel(x))
                    .ToListAsync()
            );

            return cards;
        }

        private async Task<MembershipEcardModel> CreateOffersECard(string userId)
        {
            var context = _contexFactory();
            var membershipDataPicture = context.MembershipPictureDatas
                .Where(x => x.MembershipType == (int)Declares.OfferMembership.OffersAndDiscounts)
                .FirstOrDefault();
            ApplicationUser user = context.Users.FirstOrDefault(x => x.Id == userId);
            return new MembershipEcardModel()
            {
                Id = 0,
                OwnerId = user.Id,
                Name = user.FirstName,
                Surname = user.LastName,
                ECardSequence = user.ECardSequence,
                MembershipType = ((int)Declares.OfferMembership.OffersAndDiscounts),
                IsMembershipCard = false,
                MembershipNameEng = "Offers And Discounts",
                MembershipNameAr = "العروض والخصومات",
                BackgroundPortraitUrl = membershipDataPicture.DocumentIdVerticalPicture.ToString(),
                BackgroundLandscapeUrl =
                    membershipDataPicture.DocumentIdHorizontalPicture.ToString(),
                backgroundLandscapeBackUrl =
                    membershipDataPicture.DocumentIdHorizontalBackPicture.ToString(),
                backgroundPortraitBackUrl =
                    membershipDataPicture.DocumentIdVerticalBackPicture.ToString(),
                supportArText = "للاستفسارات ، يرجى الاتصال على 027071717",
                supportEngText = "For any enquiries, please contact 027071717"
            };
        }

        private HashSet<MembershipModel> CreateHashSet(
            IEnumerable<MembershipModel> membershipModels
        )
        {
            var membershipsHashset = new HashSet<MembershipModel>();
            foreach (var membership in membershipModels)
            {
                if (membershipsHashset.Where(x => x.Id == membership.Id).Count() == 0)
                    membershipsHashset.Add(membership);
            }
            return membershipsHashset;
        }

        private Expression<Func<Models.Membership, MembershipModel>> projectToMembershipModel =
            data =>
                new MembershipModel()
                {
                    Description = data.Description,
                    Id = data.Id,
                    NameAr = data.NameAr,
                    NameEng = data.NameEng,
                    PictureDataId = data.PictureDataId,
                    PictureData = new MembershipPictureDataModel()
                    {
                        Id = data.PictureData.Id,
                        DocumentIdHorizontalPicture = data.PictureData.DocumentIdHorizontalPicture,
                        DocumentIdVerticalPicture = data.PictureData.DocumentIdVerticalPicture,
                        MembershipType = data.PictureData.MembershipType
                    }
                };

        public async Task FindMembershipCardForUserAndUpdate(MemberModel data)
        {
            if (string.IsNullOrEmpty(data.mail))
                return;
            var context = _contexFactory();
            var cards = context.MembershipECards.Where(
                x => x.isMember && x.MemberEmail == data.mail
            );
            foreach (var card in cards)
            {
                card.MemberId = data.Id;
            }
            context.MembershipECards.UpdateRange(cards);
            await context.SaveChangesAsync();
        }

        private static MembershipEcardModel MembershipModel(MembershipECard x)
        {
            return new MembershipEcardModel()
            {
                Id = x.Id,
                OwnerId = x.OwnerId,
                MemberId = x.MemberId,
                Name = x.Name,
                Surname = x.Surname,
                ECardSequence = x.ECardSequence,
                MembershipType = x.MembershipType,
                PhotoUrl = x.PhotoUrl,
                IsMembershipCard = x.IsMembershipCard,
                ValidTo = x.ValidTo,
                Status = x.Status,
                MembershipNameEng = x.Membership.NameEng,
                MembershipNameAr = x.Membership.NameAr,
                BackgroundLandscapeUrl =
                    x.Membership.PictureData.DocumentIdHorizontalPicture.ToString(),
                BackgroundPortraitUrl =
                    x.Membership.PictureData.DocumentIdVerticalPicture.ToString(),
                backgroundLandscapeBackUrl =
                    x.Membership.PictureData.DocumentIdHorizontalBackPicture.ToString(),
                backgroundPortraitBackUrl =
                    x.Membership.PictureData.DocumentIdVerticalBackPicture.ToString()
            };
        }

        public async Task DeleteExpiredMembershipECards(DateTime currentTime)
        {
            var context = _contexFactory();
            var membershipECards = context.MembershipECards.Where(x => x.ValidTo < currentTime);
            context.MembershipECards.RemoveRange(membershipECards);
            await context.SaveChangesAsync();
        }
    }
}
