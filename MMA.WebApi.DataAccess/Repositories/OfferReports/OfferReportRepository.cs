using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.OfferReports;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Models.Offer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.OfferReports
{
    public class OfferReportRepository : IOfferReportRepository
    {
        private readonly IOfferRepository _offerRepository;
        private readonly Func<MMADbContext> _contextFactory;

        public OfferReportRepository(Func<MMADbContext> contexFactory, IOfferRepository offerRepository)
        {
            _offerRepository = offerRepository;
            _contextFactory = contexFactory;
        }

        public async Task AddNewOfferReport(OfferReportModel offerReport, string userId)
        {
            var context = _contextFactory();

            var newOfferReport = new OfferReport();
            newOfferReport = PopulateDbFromDomainModel(newOfferReport, offerReport);
            newOfferReport.CreatedBy = userId;
            var offer = context.Offer.Where(x => x.Id == offerReport.OfferId).FirstOrDefault();
            if (offer != null)
            {
                offer.ReportCount++;
                context.Update(offer);
            }
            await context.OfferReport.AddAsync(newOfferReport);
            await context.SaveChangesAsync();
        }


        public async Task<IEnumerable<OfferModel>> GetAllReportedOffer(QueryModel queryModel)
        {
            var offers = _offerRepository.GetReportedOffers(queryModel);
            return offers;
        }
        public async Task<IEnumerable<OfferReportModel>> ResolveOfferReport(int reportId)
        {
            var context = _contextFactory();
            var report = context.OfferReport.Where(x => x.Id == reportId).FirstOrDefault();
            report.isResolved = true;
            report.ResolvedOn = DateTime.UtcNow;
            context.Update(report);

            var offer = context.Offer.Where(x => x.Id == report.OfferId).FirstOrDefault();

            if (offer != null)
            {
                if (offer.ReportCount > 0)
                    offer.ReportCount--;
                context.Update(offer);
            }
            context.SaveChanges();

            return await GetOfferReportByOfferId(report.OfferId);
        }

        public async Task<IEnumerable<OfferReportModel>> GetOfferReportByOfferId(int offerId)
        {
            var context = _contextFactory();

            var offersReport = await (from or in context.OfferReport
                                      join user in context.Users on or.CreatedBy equals user.Id into g1
                                      from p1 in g1.DefaultIfEmpty()
                                      where or.OfferId == offerId
                                      select new OfferReportModel()
                                      {
                                          Description = or.Description,
                                          OfferId = or.OfferId,
                                          Id = or.Id,
                                          ReportType = or.ReportType,
                                          Email = p1.Email,
                                          FirstName = p1.FirstName,
                                          LastName = p1.LastName,
                                          CreatedOn = or.CreatedOn,
                                          isResolved = or.isResolved,
                                          ResolvedOn = or.ResolvedOn
                                      }).ToListAsync();


            return offersReport;
        }

        public async Task<bool> IsReported(string userId, int reportedOfferId)
        {
            var context = _contextFactory();
            OfferReport report = null;
            report = context.OfferReport.Where(x => x.OfferId == reportedOfferId && x.CreatedBy == userId).FirstOrDefault();
            return report == null;
        }

        private OfferReport PopulateDbFromDomainModel(OfferReport entityModel, OfferReportModel data)
        {
            entityModel.CreatedOn = DateTime.UtcNow;
            entityModel.UpdatedOn = DateTime.UtcNow;
            entityModel.Description = data.Description;
            entityModel.OfferId = data.OfferId;
            entityModel.ReportType = data.ReportType;

            return entityModel;
        }

    }
}
