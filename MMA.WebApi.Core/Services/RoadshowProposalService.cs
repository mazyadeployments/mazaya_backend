using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.PDFModel;
using MMA.WebApi.Shared.Models.Response;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.Core.Services
{
    public class RoadshowProposalService : IRoadshowProposalService
    {
        private readonly IRoadshowProposalRepository _roadshowProposalRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoadshowProposalService(IRoadshowProposalRepository roadshowProposalRepository, IDocumentRepository documentRepository, UserManager<ApplicationUser> userManager)
        {
            _roadshowProposalRepository = roadshowProposalRepository;
            _documentRepository = documentRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<RoadshowProposalModel>> GetRoadshowProposals()
        {
            var roadshows = await _roadshowProposalRepository.Get().ToListAsync();

            return roadshows;
        }

        public async Task<PaginationListModel<RoadshowProposalModel>> GetAllRoadshowProposals(QueryModel queryModel, string userId)
        {
            var roles = GetUserRoles(userId);
            var roadshowProposals = await _roadshowProposalRepository.GetAllRoadshowProposals(userId, roles, queryModel).ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);

            return roadshowProposals;
        }
        public List<Roles> GetUserRoles(string userId)
        {
            ApplicationUser applicationUser = _userManager.FindByIdAsync(userId).Result;

            List<Roles> roles = new List<Roles>();
            foreach (string userRole in _userManager.GetRolesAsync(applicationUser).Result.ToList())
            {
                Enum.TryParse(userRole, out Roles role);
                roles.Add(role);
            }

            return roles;
        }

        public async Task<RoadshowProposalModel> CreateOrUpdate(RoadshowProposalModel model, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            if (model.Id == 0)
            {
                // Decode company website
                model.Company.Website = DecodeBase64String(model.Company.Website);
                model.TermsAndCondition = DecodeBase64String(model.TermsAndCondition);

                var roadshowProposal = await _roadshowProposalRepository.CreateAsync(model, auditVisitor, userId);

                return roadshowProposal;
            }

            return null;
        }

        public async Task<Maybe<RoadshowProposalModel>> GetSpecificProposalById(int id, string userId)
        {
            var roles = GetUserRoles(userId);
            var roadshowProposal = await _roadshowProposalRepository.GetSpecificProposalById(id, userId, roles);

            if (roadshowProposal != null)
            {
                var proposals = new List<RoadshowProposalModel>();
                proposals.Add(roadshowProposal);

                //ProcessComment(proposals);

                foreach (var attachment in roadshowProposal.Attachments)
                {
                    var a = await _documentRepository.GetSingleAsync(x => x.Id.ToString() == attachment.Id);
                    attachment.Name = a.Name;
                }

                return roadshowProposal;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<RoadshowProposalModel>> GetAllProposalsForCompanyCard(int id)
        {
            var companies = await _roadshowProposalRepository.GetAllProposalsForCompanyCard(id).ToListAsync();
            return companies;
        }

        public async Task<ResponseDetailsModel> DeleteProposal(int id)
        {
            return await _roadshowProposalRepository.DeleteProposal(id);
        }

        public bool CheckIfProposalIsValid(int proposalId, int companyId)
        {
            return _roadshowProposalRepository.CheckIfProposalIsValid(proposalId, companyId);
        }

        private string DecodeBase64String(string encodedString)
        {
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(data);
        }

        public async Task<AdditionalPdfInfo> GetAdditionalInfoForPdf(int companyId)
        {
            return await _roadshowProposalRepository.GetAdditionalInfoForPdf(companyId);
        }
    }
}