using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RoadshowDocuments;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.Companies;
using MMA.WebApi.Shared.Models.PDFModel;
using MMA.WebApi.Shared.Models.Response;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Repository.Offers
{
    public class RoadshowProposalRepository
        : BaseRepository<RoadshowProposalModel>,
            IRoadshowProposalRepository
    {
        private readonly IRoadshowDocumentRepository _roadshowDocumentRepository;
        private readonly IConfiguration _configuration;

        public RoadshowProposalRepository(
            IRoadshowDocumentRepository roadshowDocumentRepository,
            Func<MMADbContext> contexFactory,
            IConfiguration configuration
        )
            : base(contexFactory)
        {
            _roadshowDocumentRepository = roadshowDocumentRepository;
            _configuration = configuration;
        }

        public async Task<RoadshowProposalModel> CreateAsync(
            RoadshowProposalModel model,
            IVisitor<IChangeable> auditVisitor,
            string userId
        )
        {
            var context = ContextFactory();

            var userCompany = (
                from cs in context.CompanySuppliers
                join c in context.Company on cs.CompanyId equals c.Id
                where cs.SupplierId == userId
                select c
            ).FirstOrDefault();
            var roadshowProposal = await context.RoadshowProposal.FirstOrDefaultAsync(
                p => p.Id == model.Id && userCompany.Id == p.Company.Id
            );

            if (roadshowProposal == null)
                roadshowProposal = new RoadshowProposal();

            PopulateEntityModel(roadshowProposal, model);

            roadshowProposal.Company = userCompany;

            foreach (var roadshowProposalDocument in roadshowProposal.Documents)
            {
                roadshowProposalDocument.Accept(auditVisitor);
            }

            if (model.Id == 0)
            {
                roadshowProposal.CreatedBy = userId;
                roadshowProposal.CreatedOn = DateTime.UtcNow;
                roadshowProposal.Accept(auditVisitor);
                context.Add(roadshowProposal);
            }
            else
            {
                roadshowProposal.UpdatedBy = userId;
                roadshowProposal.UpdatedOn = DateTime.UtcNow;
                context.Update(roadshowProposal);
            }

            await context.SaveChangesAsync();

            //return projectToRoadshowModel.Compile().Invoke(roadshow);
            return model;
        }

        public async Task<RoadshowProposalModel> GetSpecificProposalById(
            int id,
            string userId,
            List<Roles> roles
        )
        {
            var context = ContextFactory();
            var roadshowProposal = new RoadshowProposalModel();

            if (roles.Contains(Roles.Supplier) || roles.Contains(Roles.SupplierAdmin))
            {
                var companySupplier = await context.CompanySuppliers
                    .Where(cs => cs.SupplierId == userId)
                    .FirstOrDefaultAsync();

                roadshowProposal = await context.RoadshowProposal
                    .Include(roadshowProposal => roadshowProposal.RoadshowVouchers)
                    .Include(roadshowProposal => roadshowProposal.Documents)
                    .Include(roadshowProposal => roadshowProposal.RoadshowOffers)
                    .AsNoTracking()
                    .Where(p => p.CompanyId.GetValueOrDefault() == companySupplier.CompanyId)
                    .Select(projectToRoadshowProposalModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            else if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                roadshowProposal = await context.RoadshowProposal
                    .Include(roadshowProposal => roadshowProposal.RoadshowVouchers)
                    .Include(roadshowProposal => roadshowProposal.Documents)
                    .Include(roadshowProposal => roadshowProposal.RoadshowOffers)
                    .AsNoTracking()
                    //.Where(p => p.Status != RoadshowProposalStatus.Draft)
                    .Select(projectToRoadshowProposalModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            else if (roles.Contains(Roles.Reviewer))
            {
                roadshowProposal = await context.RoadshowProposal
                    .Include(roadshowProposal => roadshowProposal.RoadshowVouchers)
                    .Include(roadshowProposal => roadshowProposal.Documents)
                    .Include(roadshowProposal => roadshowProposal.RoadshowOffers)
                    .AsNoTracking()
                    .Where(p => p.Status != RoadshowProposalStatus.Draft)
                    .Select(projectToRoadshowProposalModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
            }
            return roadshowProposal;
        }

        public IQueryable<Shared.Models.Roadshow.RoadshowProposalModel> Get()
        {
            var context = ContextFactory();

            return context.RoadshowProposal.Select(projectToRoadshowProposalModel);
        }

        protected override IQueryable<RoadshowProposalModel> GetEntities()
        {
            var context = ContextFactory();

            return context.RoadshowProposal.Select(projectToRoadshowProposalModel);
        }

        public IQueryable<RoadshowProposalModel> GetAllRoadshowProposals(
            string userId,
            List<Roles> roles,
            QueryModel queryModel
        )
        {
            var context = ContextFactory();
            IQueryable<RoadshowProposal> roadshowProposals =
                context.RoadshowProposal.AsNoTracking();

            if (roles.Contains(Roles.AdnocCoordinator) || roles.Contains(Roles.Admin))
            {
                // Admin can see all roadshow proposals
                //roadshowProposals = roadshowProposals;
            }
            else if (roles.Contains(Roles.SupplierAdmin) || roles.Contains(Roles.Supplier))
            {
                roadshowProposals = (
                    from r in roadshowProposals
                    join cs in context.CompanySuppliers on r.Company.Id equals cs.CompanyId
                    where cs.SupplierId == userId
                    select r
                );
            }

            var filteredRoadshow = Filter(roadshowProposals, queryModel);
            var roadshowModels = filteredRoadshow.Select(projectToRoadshowProposalCardModel);

            return Sort(queryModel.Sort, roadshowModels);
        }

        private static IQueryable<RoadshowProposal> Filter(
            IQueryable<RoadshowProposal> roadshowProposals,
            QueryModel queryModel
        )
        {
            var filteredRoadshowProposals = roadshowProposals.Where(
                roadshowProposal =>
                    roadshowProposal.Subject
                        .ToLower()
                        .Contains(queryModel.Filter.Keyword.Trim().ToLower())
            );

            if (queryModel.Filter.Status?.Any() == true)
            {
                var filteredStatuses = Enum.GetValues(typeof(RoadshowProposalStatus))
                    .Cast<RoadshowProposalStatus>()
                    .Where(rs => queryModel.Filter.Status.Contains(rs.ToString()))
                    .ToList();

                filteredRoadshowProposals = filteredRoadshowProposals.Where(
                    r => filteredStatuses.Contains(r.Status)
                );
            }

            return filteredRoadshowProposals;
        }

        private static IQueryable<RoadshowProposalModel> Sort(
            SortModel sortModel,
            IQueryable<RoadshowProposalModel> roadshows
        )
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return roadshows.OrderByDescending(x => x.ExpiryDate);
                }
                else
                {
                    return roadshows.OrderBy(x => x.ExpiryDate);
                }
            }
            else
            {
                return roadshows.OrderByDescending(x => x.ExpiryDate);
            }
        }

        public IQueryable<RoadshowProposalModel> GetAllProposalsForCompanyCard(int id)
        {
            var context = ContextFactory();
            return from p in context.RoadshowProposal.Where(
                    p => p.Company.Id == id && p.Status == RoadshowProposalStatus.Active
                )
                select new RoadshowProposalModel
                {
                    Id = p.Id,
                    Subject = p.Subject,
                    CreatedOn = p.CreatedOn,
                    Name = p.Name
                };
        }

        public async Task<ResponseDetailsModel> DeleteProposal(int id)
        {
            var context = ContextFactory();

            var isAttachedToOffer = await (
                from rp in context.RoadshowProposal
                join ro in context.RoadshowOffer on rp.Id equals ro.RoadshowProposalId
                where ro.RoadshowProposalId == id
                select rp
            ).AnyAsync();

            if (isAttachedToOffer)
                return new ResponseDetailsModel
                {
                    Description =
                        "There is offer attached to this proposal, therefore it can't be deleted.",
                    Message = "Proposal can't be deleted.",
                    StatusCode = (int)HttpStatusCode.BadRequest
                };

            var proposal = await context.RoadshowProposal
                .Include(rp => rp.RoadshowVouchers)
                .Include(rp => rp.Documents)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (proposal != null)
            {
                context.RoadshowVoucher.RemoveRange(proposal.RoadshowVouchers);
                context.RoadshowOfferProposalDocument.RemoveRange(proposal.Documents);
                context.RoadshowProposal.Remove(proposal);
                await context.SaveChangesAsync();
                return new ResponseDetailsModel
                {
                    Description = "Roadshow proposal successfully deleted",
                    Message = "Proposal deleted.",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            return new ResponseDetailsModel
            {
                Description = "Roadshow proposal with that ID doesn't exists in database.",
                Message = "Proposal can't be deleted.",
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        private Expression<
            Func<RoadshowProposal, RoadshowProposalModel>
        > projectToRoadshowProposalModel = data =>
            new RoadshowProposalModel()
            {
                Id = data.Id,
                // For now created proposal will directly go to active status
                Status = RoadshowProposalStatus.Active, //data.Status,
                RoadshowDetails = data.RoadshowDetails,
                EquipmentItem = data.EquipmentItem,
                Subject = data.Subject,
                Company = new Shared.Models.Companies.CompanyModel
                {
                    CompanyNationality = data.Company.CompanyNationality,
                    EstablishDate = data.Company.EstablishDate.SpecifyKind(DateTimeKind.Utc),
                    ExpiryDate = data.Company.ExpiryDate.SpecifyKind(DateTimeKind.Utc),
                    Fax = string.IsNullOrWhiteSpace(data.Company.FaxE164Number)
                        ? null
                        : new PhoneNumberModel
                        {
                            CountryCode = data.Company.FaxCountryCode,
                            E164Number = data.Company.FaxE164Number,
                            InternationalNumber = data.Company.Fax,
                            Number = data.Company.FaxNumber,
                        },
                    IDforADCCI = data.Company.IDforADCCI,
                    Instagram = data.Company.Instagram,
                    Land = string.IsNullOrWhiteSpace(data.Company.LandE164Number)
                        ? null
                        : new PhoneNumberModel
                        {
                            CountryCode = data.Company.LandCountryCode,
                            E164Number = data.Company.LandE164Number,
                            InternationalNumber = data.Company.Land,
                            Number = data.Company.LandNumber,
                        },
                    LegalForm = data.Company.LegalForm,
                    LicenseNo = data.Company.LicenseNo,
                    Logo =
                        data.Company.Logo != null
                            ? data.Company.Logo.DocumentId.ToString()
                            : string.Empty,
                    Mobile = string.IsNullOrWhiteSpace(data.Company.MobileE164Number)
                        ? null
                        : new PhoneNumberModel
                        {
                            CountryCode = data.Company.MobileCountryCode,
                            E164Number = data.Company.MobileE164Number,
                            InternationalNumber = data.Company.Mobile,
                            Number = data.Company.MobileNumber,
                        },
                    NameArabic = data.Company.NameArabic,
                    NameEnglish = data.Company.NameEnglish,
                    OfficialEmail = data.Company.OfficialEmail,
                    POBox = data.Company.POBox,
                    Facebook = data.Company.Facebook,
                    Website = data.Company.Website,
                    CreatedBy = data.CreatedBy,
                    CreatedOn = data.CreatedOn,
                    UpdatedBy = data.UpdatedBy,
                    UpdatedOn = data.UpdatedOn,
                    Id = data.Company.Id,
                },
                TermsAndCondition = data.TermsAndCondition,
                TermsAndConditionChecked = data.TermsAndConditionChecked,
                OfferEffectiveDate = data.OfferEffectiveDate.SpecifyKind(DateTimeKind.Utc),
                ExpiryDate = data.ExpiryDate.SpecifyKind(DateTimeKind.Utc),
                Name = data.Name,
                Title = data.Title,
                Signature = data.Signature,
                Manager = data.Manager,
                CreatedOn = data.CreatedOn,
                CreatedBy = data.CreatedBy,
                UpdatedOn = data.UpdatedOn,
                UpdatedBy = data.UpdatedBy,
                OffersCount = data.RoadshowOffers.Count,
                Documents = data.Documents
                    .Where(d => d.Type != OfferDocumentType.QRCode)
                    .Select(
                        d =>
                            new RoadshowOfferProposalDocumentModel
                            {
                                Id = d.Id,
                                RoadshowProposalId = d.RoadshowOfferProposalId,
                                DocumentId = d.DocumentId,
                                Type = d.Type,
                                OriginalImageId = d.DocumentId
                            }
                    )
                    .ToList(),
                RoadshowVouchers = data.RoadshowVouchers
                    .Select(
                        v =>
                            new RoadshowVoucherModel
                            {
                                Id = v.Id,
                                Quantity = v.Quantity,
                                Validity = v.Validity,
                                Details = v.Details
                            }
                    )
                    .ToList(),
                //RoadshowOfferModels = data.RoadshowOffers.Select(o => new RoadshowOfferModel
                //{

                //}).ToList(),
                Attachments = data.Documents
                    .Where(d => d.Type == OfferDocumentType.Document)
                    .Select(
                        d =>
                            new Shared.Models.Attachment.AttachmentModel
                            {
                                Id = d.DocumentId.ToString(),
                                Name = d.Document.Name,
                                Type = OfferDocumentType.Document.ToString()
                            }
                    )
                    .ToList()
            };

        private void PopulateEntityModel(RoadshowProposal data, RoadshowProposalModel model)
        {
            data.Id = model.Id;
            data.Documents = model.Attachments
                .Select(
                    rpd =>
                        new RoadshowOfferProposalDocument
                        {
                            RoadshowOfferProposalId = model.Id,
                            DocumentId = new Guid(rpd.Id),
                            CreatedOn = DateTime.UtcNow,
                            CreatedBy = model.CreatedBy,
                            UpdatedOn = DateTime.UtcNow,
                            UpdatedBy = model.UpdatedBy,
                            Type = OfferDocumentType.Document
                        }
                )
                .ToList();
            data.RoadshowVouchers = model.RoadshowVouchers
                .Select(
                    rv =>
                        new RoadshowVoucher
                        {
                            RoadshowOfferId = null,
                            RoadshowProposalId = model.Id,
                            Quantity = rv.Quantity,
                            Details = rv.Details,
                            Validity = rv.Validity
                        }
                )
                .ToList();
            data.Status = RoadshowProposalStatus.Active;
            data.RoadshowDetails = model.RoadshowDetails;
            data.EquipmentItem = model.EquipmentItem;
            data.Subject = model.Subject;
            data.TermsAndCondition = model.TermsAndCondition;
            data.TermsAndConditionChecked = model.TermsAndConditionChecked;
            data.OfferEffectiveDate = model.OfferEffectiveDate;
            data.ExpiryDate = model.ExpiryDate;
            data.Name = model.Name;
            data.Title = model.Title;
            data.Signature = model.Signature;
            data.Manager = model.Manager;
            data.CreatedOn = model.CreatedOn;
            data.CreatedBy = model.CreatedBy;
            data.UpdatedOn = model.UpdatedOn;
            data.UpdatedBy = model.UpdatedBy;
        }

        private readonly Expression<
            Func<RoadshowProposal, RoadshowProposalModel>
        > projectToRoadshowProposalCardModel = data =>
            new RoadshowProposalModel()
            {
                Subject = data.Subject,
                Status = data.Status,
                Title = data.Title,
                ExpiryDate = data.ExpiryDate,
                Id = data.Id,
                OffersCount = data.RoadshowOffers.Count
            };

        public async Task DeactivateRoadshowProposals(int companyId)
        {
            var context = ContextFactory();

            var roadshowProposalIds = context.RoadshowProposal
                .Where(oa => oa.Company.Id == companyId)
                .Select(x => x.Id)
                .ToList();
            var roadshowProposals = context.RoadshowProposal
                .Where(x => roadshowProposalIds.Contains(x.Id))
                .ToList();
            roadshowProposals.ForEach(o => o.Status = RoadshowProposalStatus.Deactivated);

            context.RoadshowProposal.UpdateRange(roadshowProposals);
            await context.SaveChangesAsync();
        }

        public async Task HardOfCompanyDeleteProposals(int companyId)
        {
            var context = ContextFactory();

            var roadshowProposalIds = context.RoadshowProposal
                .Where(oa => oa.Company.Id == companyId)
                .Select(x => x.Id)
                .ToList();
            var roadshowProposals = context.RoadshowProposal
                .Where(x => roadshowProposalIds.Contains(x.Id))
                .ToList();

            context.RoadshowProposal.RemoveRange(roadshowProposals);
            await context.SaveChangesAsync();
        }

        public bool CheckIfProposalIsValid(int id, int companyId)
        {
            var context = ContextFactory();

            var proposal = context.RoadshowProposal
                .Where(
                    rp =>
                        rp.Id == id
                        && rp.Company.Id == companyId
                        && rp.ExpiryDate.Date > DateTime.UtcNow.Date
                )
                .FirstOrDefault();

            return proposal == null ? false : true;
        }

        public async Task<AdditionalPdfInfo> GetAdditionalInfoForPdf(int companyId)
        {
            var context = ContextFactory();
            return await (
                from c in context.Company
                join cs in context.CompanySuppliers on c.Id equals cs.CompanyId
                join u in context.Users on cs.SupplierId equals u.Id
                where c.Id == companyId
                select new AdditionalPdfInfo
                {
                    FocalPoints = c.CompanySuppliers.Select(
                        cs =>
                            new ApplicationUserModel
                            {
                                FirstName = cs.Supplier.FirstName,
                                LastName = cs.Supplier.LastName,
                                Title = cs.Supplier.Title,
                                PhoneNumber = cs.Supplier.PhoneNumber
                            }
                    ),
                    CompanyLocations = c.CompanyLocations.Select(
                        cl =>
                            new CompanyLocationModel
                            {
                                Address = cl.Address,
                                Country = cl.Country,
                                Vicinity = cl.Vicinity
                            }
                    )
                }
            ).FirstOrDefaultAsync();
        }
    }
}
