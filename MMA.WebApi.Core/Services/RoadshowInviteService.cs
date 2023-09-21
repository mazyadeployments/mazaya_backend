using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.MailStorage;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.RoadshowInvite;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.ApplicationUser;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.MailStorage;
using MMA.WebApi.Shared.Models.Roadshow;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Core.Services
{
    public class RoadshowInviteService : IRoadshowInviteService
    {
        private readonly IConfiguration _configuration;
        private readonly IRoadshowInviteRepository _roadshowInviteRepository;
        private readonly IRoadshowRepository _roadshowRepository;
        private readonly IRoleService _roleService;
        private readonly IMailStorageService _mailStorageServiceService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoadshowInviteService(IRoadshowInviteRepository roadshowInviteRepository, IRoadshowRepository roadshowRepository,
                                        IMailStorageService mailStorageServiceService, IConfiguration configuration,
                                        UserManager<ApplicationUser> userManager, IRoleService roleService)
        {
            _configuration = configuration;
            _roadshowInviteRepository = roadshowInviteRepository;
            _roadshowRepository = roadshowRepository;
            _mailStorageServiceService = mailStorageServiceService;
            _roleService = roleService;
            _userManager = userManager;
        }

        public async Task<RoadshowInviteModel> Update(RoadshowInviteModel model, string userId)
        {
            var roadshowInviteModel = await _roadshowInviteRepository.Update(model, userId);

            return roadshowInviteModel;
        }
        public async Task SendRoadshowInvitesFromModal(RoadshowInvitesQueryModel roadshowInvitesQueryModel, int id, string userId)
        {
            await _roadshowInviteRepository.SendRoadshowInvitesFromModal(roadshowInvitesQueryModel, id, userId);
        }

        public async Task SendRoadshowInvitesFromForm(RoadshowInvitesQueryModel roadshowInvitesQueryModel, int id, string userId)
        {
            await _roadshowInviteRepository.SendRoadshowInvitesFromForm(roadshowInvitesQueryModel, id, userId);
        }

        public int AddOrUpdateRoadshowEventToRoadshowInvite(RoadshowEventModel roadshowEventModel, int id, int idinvite)
        {
            int eventId = _roadshowInviteRepository.AddOrUpdateRoadshowEventToRoadshowInvite(roadshowEventModel, id, idinvite);
            return eventId;
        }
        public async Task<bool> DeleteRoadshowEventToRoadshowInvite(int idevent)
        {
            return await _roadshowInviteRepository.DeleteRoadshowEventToRoadshowInvite(idevent);
        }

        public void DeleteRoadshowInvites(RoadshowInvitesQueryModel roadshowInvitesQueryModel)
        {
            _roadshowInviteRepository.DeleteRoadshowInvites(roadshowInvitesQueryModel);
        }

        public void UpdateRoadshowInvites(RoadshowInvitesQueryModel roadshowInvitesQueryModel)
        {
            _roadshowInviteRepository.UpdateRoadshowInvites(roadshowInvitesQueryModel);
        }

        public async Task<PaginationListModel<RoadshowInviteModel>> GetAllRoadshowInvitesForRoadshow(QueryModel queryModel, string userId, int roadshowId)
        {
            var roadshowInviteModels = _roadshowInviteRepository.GetAllRoadshowInvitesForRoadshow(queryModel, userId, roadshowId);

            return await roadshowInviteModels.ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);
        }
        public async Task<RoadshowInviteDetailsModel> GetRoadshowInviteDetails(int roadshowId, int inviteId, string userId)
        {
            var roles = await _roleService.GetUserRoles(userId);
            var roadshowInviteModel = await _roadshowInviteRepository.GetRoadshowInviteDetails(roadshowId, inviteId, roles, userId);

            if (roadshowInviteModel != null)
            {
                await ProcessComment(roadshowInviteModel);
            }

            return roadshowInviteModel;
        }

        public async Task UpdateOfRoadshowInviteDetails(RoadshowInviteDetailsModel roadshowInviteDetailsModel, string userId)
        {
            await _roadshowInviteRepository.UpdateOfRoadshowInviteDetails(roadshowInviteDetailsModel, userId);
            await CheckIfEmailNeedsToBeSent(roadshowInviteDetailsModel);
        }

        private async Task CheckIfEmailNeedsToBeSent(RoadshowInviteDetailsModel roadshowInviteDetailsModel)
        {
            switch (roadshowInviteDetailsModel.Status)
            {
                case RoadshowInviteStatus.Approved:
                    await SendEmailsToSupplierAdmins(MessageTemplateList.Roadshow_Approved_Notify_SupplierAdminOrSupplier, roadshowInviteDetailsModel);
                    break;
                case RoadshowInviteStatus.Returned:
                    await SendEmailsToSupplierAdmins(MessageTemplateList.Roadshow_Returned_Notify_SupplierAdminOrSupplier, roadshowInviteDetailsModel);
                    break;
                case RoadshowInviteStatus.Renegotiation:
                    await SendEmailsToCoordinatorOrAdmin(MessageTemplateList.Roadshow_Invite_Renegotiation_Notify_SupplierAdminOrSupplier, roadshowInviteDetailsModel);
                    break;
            }
        }

        private async Task SendEmailsToSupplierAdmins(MessageTemplateList messageTemplate, RoadshowInviteDetailsModel roadshowInviteDetailsModel)
        {
            var suppliers = await _roadshowInviteRepository.GetSupplierAdminsForInvite(roadshowInviteDetailsModel.Id);
            var roadshow = _roadshowRepository.Get().Where(r => r.Id == roadshowInviteDetailsModel.RoadshowId).FirstOrDefault();

            var locations = roadshow.Locations.Select(l => l.Title).ToList();
            var loc = locations.Aggregate((current, next) => current + ", " + next);

            suppliers.ForEach(s =>
            {
                var emailData = new EmailDataModel()
                {
                    User = s,
                    MailTemplateId = messageTemplate,
                    RoadshowName = roadshow.Title,
                    RoadshowLocation = loc,
                    CompanyName = s.CompanyName
                };

                _mailStorageServiceService.CreateMail(emailData);
            });
        }

        private async Task SendEmailsToCoordinatorOrAdmin(MessageTemplateList messageTemplate, RoadshowInviteDetailsModel roadshowInviteDetailsModel)
        {
            var admins = await _userManager.GetUsersInRoleAsync("ADNOC Coordinator");
            var roadshow = _roadshowRepository.Get().Where(r => r.Id == roadshowInviteDetailsModel.RoadshowId).FirstOrDefault();
            var companyName = _roadshowInviteRepository.Get().Where(ri => ri.Id == roadshowInviteDetailsModel.Id).Select(ri => ri.Company.NameEnglish).FirstOrDefault();

            var locations = roadshow.Locations.Select(l => l.Title).ToList();
            var loc = locations.Aggregate((current, next) => current + ", " + next);

            admins.ToList().ForEach(a =>
            {
                ApplicationUserModel userModel = new ApplicationUserModel()
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Active = a.Active,
                    Title = a.Title,
                    Email = _configuration["Emails:SysAdminMail"]
                };

                var emailData = new EmailDataModel()
                {
                    User = userModel,
                    MailTemplateId = messageTemplate,
                    RoadshowLocation = loc,
                    RoadshowName = roadshow.Title,
                    CompanyName = companyName
                };

                _mailStorageServiceService.CreateMail(emailData);
            });
        }

        private async Task ProcessComment(RoadshowInviteDetailsModel invite)
        {
            // Get text for deleted user from config file
            var deletedUser = _configuration["DeletedUser"];
            invite.RoadshowComments = invite.RoadshowComments.OrderByDescending(x => x.CreatedOn).ToList();
            foreach (var comment in invite.RoadshowComments)
            {
                await _mailStorageServiceService.SetCreatedByName(deletedUser, comment);
            }
        }
    }
}