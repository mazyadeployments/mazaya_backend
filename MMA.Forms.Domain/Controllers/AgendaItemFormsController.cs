using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMA.WebApi.Shared.Interfaces.AgendaItemSections;
using MMA.WebApi.Shared.Models;
using MMA.WebApi.Shared.Models.AgendaItemSections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.Forms.Domain.Controllers
{
    //[Route("[controller]")]
    [Authorize("Bearer")]
    [ApiController]

    public class AgendaItemFormsController : BaseController
    {

        private IAgendaItemFormsService agendaItemFormsService;

        public AgendaItemFormsController(IAgendaItemFormsService agendaItemFormsService)
        {
            this.agendaItemFormsService = agendaItemFormsService;
        }

         

        /// <summary>
        /// returns sections to be rendered for this agenda item
        /// </summary>
        /// <param name="agendaItemId"></param>
        /// <returns></returns>
        [HttpGet("agendaitem/{agendaItemId}/formsections")]
        public async Task<IActionResult> GetFormSectionsList(int agendaItemId)
        {
            List<SectionModel> section = agendaItemFormsService.GetSections(agendaItemId).Result;
            return Ok(section);
        }


        /// <summary>
        /// get data for specific section for provided agenda item
        /// </summary>
        /// <param name="agendaItemId"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        [HttpGet("agendaitem/{agendaItemId}/sections/{sectionId}")]
        public async Task<IActionResult> GetSections(int agendaItemId, AgendaItemSectionsList sectionId)
        {
            var section = await agendaItemFormsService.Get(agendaItemId, sectionId);
            return Ok(section);
        }


        #region save sections 


        /// <summary>
        /// GENERAL_INFO = 5
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("agendaitem/savesection/s5")]
        public async Task<IActionResult> SaveSectionsGeneralInfo(SectionGeneralInfoModel model)
        {
            string userId = this.UserId;
            await agendaItemFormsService.Save(model, userId);
            return Ok();
        }

        /// <summary>
        ///  SUBJECT = 10
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("agendaitem/savesection/s10")]
        public async Task<IActionResult> SaveSectionsSubject(SectionSubjectModel model)
        {
            string userId = this.UserId;
            await agendaItemFormsService.Save(model, userId);
            return Ok();
        }

        /// <summary>
        ///  BUDGET = 20
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("agendaitem/savesection/s20")]
        public async Task<IActionResult> SaveSectionsBudget(SectionBudgetModel model)
        {
            string userId = this.UserId;
             await agendaItemFormsService.Save(model, userId);
            return Ok();
        }

        /// <summary>
        /// BIDDER_LIST_BL = 30
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("agendaitem/savesection/bidderlist")]
        public async Task<IActionResult> SaveBidderSectionAW(SectionBidderListModel model)
        {
            string userId = this.UserId;
            await agendaItemFormsService.Save(model, userId);
            return Ok();
        }

     


        /// <summary>
        ///  INTODUCTIOIN_BACKGROUND = 60
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("agendaitem/savesection/s60")]
        public async Task<IActionResult> SaveSectionsIntoductions(SectionIntroductionModel model)
        {
            string userId = this.UserId;
            await agendaItemFormsService.Save(model, userId);
            return Ok();
        }


        /// <summary>
        ///  TENDER_STRATEGY = 70
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("agendaitem/savesection/s70")]
        public async Task<IActionResult> SaveSectionsTenderStrategy(SectionTenderStrategyModel model)
        {
            string userId = this.UserId;
            await agendaItemFormsService.Save(model, userId);
            return Ok();
        }


        /// <summary>
        ///  CLARIFICATION = 80
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("agendaitem/savesection/s80")]
        public async Task<IActionResult> SaveSectionsClarification(SectionClarificationModel model)
        {
            string userId = this.UserId;
            await agendaItemFormsService.Save(model, userId);
            return Ok();
        }


        /// <summary>
        ///  RECOMMENDATION = 90
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("agendaitem/savesection/s90")]
        public async Task<IActionResult> SaveSectionsReconmendation(SectionRecommendationModel model)
        {
            string userId = this.UserId;
            await agendaItemFormsService.Save(model, userId);
            return Ok();
        }

        #endregion


    }

}
