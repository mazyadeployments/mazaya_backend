using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.Helpers;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Interfaces.ProposalPDFCreatorService;
using MMA.WebApi.Shared.Models.Roadshow;
using System.Threading.Tasks;

namespace MMA.WebApi.Controllers
{

    [Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]
    public class PDFCreatorController : BaseController
    {
        private readonly IProposalPDFCreatorService _proposalService;
        private readonly IRoadshowProposalService _roadshowProposalService;
        private readonly IConfiguration _configuration;

        public PDFCreatorController(IProposalPDFCreatorService proposalService,
                                IRoadshowProposalService roadshowProposalService,
                                IConfiguration configuration)
        {
            _proposalService = proposalService;
            _roadshowProposalService = roadshowProposalService;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("proposal/{proposalId}/{zone}")]
        public async Task<IActionResult> GetPDFProposal(int proposalId, int zone)
        {
            //API SECURITY: CHECK THIS ONE!!!
            var proposal = await _proposalService.GetProposalsData(proposalId);
            proposal = AddTimeZoneProposal(proposal, zone);
            var additionalInfoAboutPDF = await _roadshowProposalService.GetAdditionalInfoForPdf(proposal.Company.Id);
            proposal.Company.Suppliers = additionalInfoAboutPDF.FocalPoints;
            proposal.Company.CompanyLocations = additionalInfoAboutPDF.CompanyLocations;
            var result = ProposalPDFGenerator.GenerateProposalPDF(proposal, _configuration);
            return Ok(result);
        }

        private RoadshowProposalModel AddTimeZoneProposal(RoadshowProposalModel model, int zone)
        {
            zone = zone * -1;
            foreach (var i in model.RoadshowVouchers)
            {
                i.Validity = i.Validity.AddMinutes(zone);
            }

            model.OfferEffectiveDate = model.OfferEffectiveDate.AddMinutes(zone);
            model.ExpiryDate = model.ExpiryDate.AddMinutes(zone);
            model.RoadshowVouchers = model.RoadshowVouchers;
            model.Company.ExpiryDate = model.Company.ExpiryDate.AddMinutes(zone);
            model.Company.EstablishDate = model.Company.EstablishDate.AddMinutes(zone);
            return model;
        }
    }
}