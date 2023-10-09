using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.ApplicationUsers;
using MMA.WebApi.Shared.Interfaces.OfferReports;
using MMA.WebApi.Shared.Interfaces.UserNotifications;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Offer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class OfferReportService : IOfferReportService
    {
        private readonly IOfferReportRepository _offerReportRepository;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IUserNotificationService _userNotificationService;
        public OfferReportService(IOfferReportRepository offerReportRepository, IApplicationUserService applicationUserService, IUserNotificationService userNotificationService)
        {
            _offerReportRepository = offerReportRepository;
            _applicationUserService = applicationUserService;
            _userNotificationService = userNotificationService;
        }

        public async Task<IEnumerable<OfferReportModel>> GetReportsByOfferId(int offerId)
        {
            return await _offerReportRepository.GetOfferReportByOfferId(offerId);
        }

        public async Task SaveAsync(OfferReportModel data, string userId)
        {
            await _offerReportRepository.AddNewOfferReport(data, userId);
            //dobaviti usere za notifikacuju

            var usersForNotification = _applicationUserService.GetUsersForOfferReport();

            //kreirati notifikacij
            await _userNotificationService.CreateNotificationForOfferReport(data.OfferId, usersForNotification);
            //obavestiti sys admine za report

        }
        public async Task<PaginationListModel<OfferModel>> GetAllReportedOffer(QueryModel queryModel)
        {

            var offers = await _offerReportRepository.GetAllReportedOffer(queryModel);
            return offers.ToPagedList(queryModel.Page, queryModel.PaginationParameters.PageSize);
            /*
            var invitations = await _applicationUserRepository.GetAllUserInvitationsPaginated(queryModel);
            var count = invitations.ToList().Count;
            return invitations.ToPagedList(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);
             */
        }

        public Task<bool> IsReported(string userId, int reportedOfferId)
        {
            return _offerReportRepository.IsReported(userId, reportedOfferId);
        }

        public Task<IEnumerable<OfferReportModel>> ResolveOfferReport(int reportId)
        {
            return _offerReportRepository.ResolveOfferReport(reportId);
        }
    }
}
