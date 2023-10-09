using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Helpers;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.Roles;
using MMA.WebApi.Shared.Models.PDFModel;
using MMA.WebApi.Shared.Models.Roadshow;
using MMA.WebApi.Shared.Monads;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class RoadshowProposalController : BaseController
    {
        private readonly IRoadshowProposalService _roadshowProposalService;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        public RoadshowProposalController(IRoadshowProposalService roadshowProposalService, IRoleService roleService,
            IConfiguration configuration)
        {
            _roadshowProposalService = roadshowProposalService;
            _roleService = roleService;
            _configuration = configuration;
        }

        [HttpPost()]
        public async Task<IActionResult> CreateOrUpdate(RoadshowProposalModel model)
        {
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                var result = await _roadshowProposalService.CreateOrUpdate(model, UserId);

                if (result is null)
                    return NotFound("Roadshow proposal not found");

                return Ok(result);
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        [HttpPost("generate-pdf/{zone}")]
        public async Task<IActionResult> GeneratePDF(ProposalPDFModel pdfModel, int zone)
        {
            //API SECURITY: CHECK THIS ONE!!!
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                var model = MapPDFModelToAgreementModel(pdfModel, zone);
                model.Company.Website = Encoding.UTF8.GetString(Convert.FromBase64String(model.Company.Website));
                model.TermsAndCondition = Encoding.UTF8.GetString(Convert.FromBase64String(model.TermsAndCondition));
                var additionalInfoAboutPDF = await _roadshowProposalService.GetAdditionalInfoForPdf(model.Company.Id);
                model.Company.Suppliers = additionalInfoAboutPDF.FocalPoints;
                model.Company.CompanyLocations = additionalInfoAboutPDF.CompanyLocations;

                var result = ProposalPDFGenerator.GenerateProposalPDF(model, _configuration);
                return Ok(result);
            }

            return Unauthorized("You do not have permission to do this action. Please contact system administartor.");
        }



        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            //API SECURITY: CHECK THIS ONE!!!
            return Ok(await _roadshowProposalService.GetRoadshowProposals());
        }

        [HttpPost("page/{pageNumber}")]
        public async Task<IActionResult> GetAllRoadshowProposals(QueryModel queryModel, int pageNumber)
        {
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                queryModel.PaginationParameters = new PaginationParameters { PageNumber = pageNumber };
                return Ok(await _roadshowProposalService.GetAllRoadshowProposals(queryModel, UserId));
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        [HttpGet("specific/{id}")]
        public async Task<IActionResult> GetSpecificProposalById(int id)
        {
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                var result = await _roadshowProposalService.GetSpecificProposalById(id, UserId);

                if (result is Maybe<RoadshowProposalModel>.None || result == null)
                    return NotFound("Roadshow Proposal not found");

                return Ok(result.Value);
            }

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }


        [HttpGet("company/{id}")]
        public async Task<IActionResult> GetAllProposalsForCompanyCard(int id)
        {
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
                return Ok(await _roadshowProposalService.GetAllProposalsForCompanyCard(id));

            return BadRequest("You do not have permission to do this action. Please contact system administartor.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProposal(int id)
        {
            if (_roleService.CheckIfUserIsAdminOrSupplier(UserId))
            {
                var response = await _roadshowProposalService.DeleteProposal(id);
                if (response.IsSuccessStatusCode)
                    return Ok(JsonConvert.SerializeObject(response.Description));

                return BadRequest(JsonConvert.SerializeObject(response.Description));
            }

            return Unauthorized("You do not have permission to do this action. Please contact system administartor.");
        }

        private RoadshowProposalModel MapPDFModelToAgreementModel(ProposalPDFModel pdfModel, int zone)
        {
            zone = zone * -1;
            foreach (var i in pdfModel.RoadshowVouchers)
            {
                i.Validity = i.Validity.AddMinutes(zone);
            }

            return new RoadshowProposalModel()
            {
                Subject = pdfModel.Subject,
                Status = pdfModel.Status,
                Company = pdfModel.Company,
                RoadshowVouchers = pdfModel.RoadshowVouchers,
                TermsAndConditionChecked = pdfModel.TermsAndConditionChecked,
                TermsAndCondition = pdfModel.TermsAndCondition,
                OfferEffectiveDate = pdfModel.OfferEffectiveDate.AddMinutes(zone),
                ExpiryDate = pdfModel.ExpiryDate.AddMinutes(zone),
                Name = pdfModel.Name,
                Title = pdfModel.Title,
                Signature = pdfModel.Signature,
                Manager = pdfModel.Manager,
            };
        }
    }
}