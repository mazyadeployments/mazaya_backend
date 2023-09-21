using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Companies;
using MMA.WebApi.Shared.Interfaces.RoadshowEvent;
using MMA.WebApi.Shared.Models.Roadshow;
using System;
using System.Linq;
using System.Linq.Expressions;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.Repository.RoadshowEvents
{
    public class RoadshowEventRepository : BaseRepository<RoadshowEventModel>, IRoadshowEventRepository
    {
        private readonly ICompanyRepository _companyRepository;
        public RoadshowEventRepository(Func<MMADbContext> contexFactory, ICompanyRepository companyRepository) : base(contexFactory)
        {
            _companyRepository = companyRepository;
        }
        public IQueryable<RoadshowOfferModel> GetOffersForSelectedEvent(QueryModel queryModel, int eventId)
        {
            var context = ContextFactory();
            IQueryable<RoadshowOffer> roadshowInvites = context.RoadshowEventOffer
                                                    .AsNoTracking()
                                                    .Include(re => re.RoadshowOffer).ThenInclude(roadshowOffers => roadshowOffers.OfferDocuments)
                                                    .Include(re => re.RoadshowOffer).ThenInclude(roadshowOffers => roadshowOffers.RoadshowOfferTags)
                                                    .Where(re => re.RoadshowEventId == eventId)
                                                    .Select(re => re.RoadshowOffer)
                                                    .Where(o => o.RoadshowProposal.Status == RoadshowProposalStatus.Active);


            var filteredRoadshowInvites = Filter(roadshowInvites, queryModel);
            var roadshowOfferModels = filteredRoadshowInvites.Select(projectToRoadshowOfferCardModel);

            return Sort(queryModel.Sort, roadshowOfferModels);
        }

        private static IQueryable<RoadshowOffer> Filter(IQueryable<RoadshowOffer> roadshowOffers, QueryModel queryModel)
        {

            var filteredRoadshowOffers = roadshowOffers.Where(roadshowOffer => roadshowOffer.Title.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower()) || roadshowOffer.Description.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower()));

            if (queryModel.Filter.Status?.Any() == true)
            {
                var filteredStatuses = Enum.GetValues(typeof(RoadshowOfferStatus)).Cast<RoadshowOfferStatus>().Where(rs => queryModel.Filter.Status.Contains(rs.ToString())).ToList();

                filteredRoadshowOffers = filteredRoadshowOffers.Where(r => filteredStatuses.Contains(r.Status));
            }

            return filteredRoadshowOffers;
        }

        private static IQueryable<RoadshowOfferModel> Sort(SortModel sortModel, IQueryable<RoadshowOfferModel> roadshowOffers)
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return roadshowOffers.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return roadshowOffers.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return roadshowOffers.OrderByDescending(x => x.UpdatedOn);
            }
        }
        private readonly Expression<Func<RoadshowOffer, RoadshowOfferModel>> projectToRoadshowOfferCardModel = data =>
          new RoadshowOfferModel()
          {
              Id = data.Id,
              RoadshowProposalId = data.RoadshowProposalId,
              MainImage = data.OfferDocuments.Select(d => d.OriginalImageId.ToString()).FirstOrDefault(),
              Tag = data.RoadshowOfferTags.Select(t => t.Tag.Title).FirstOrDefault(),
              Title = data.Title,
              Description = data.Description,
              //Status = data.Status,
              CreatedOn = data.CreatedOn,
              CreatedBy = data.CreatedBy,
              UpdatedBy = data.UpdatedBy,
              UpdatedOn = data.UpdatedOn
          };

        public IQueryable<RoadshowEventModel> Get()
        {
            throw new NotImplementedException();
        }


        protected override IQueryable<RoadshowEventModel> GetEntities()
        {
            throw new NotImplementedException();
        }
    }
}
