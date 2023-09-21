using MMA.WebApi.Shared.Interfaces.AgendaItems;
using MMA.WebApi.Shared.Interfaces.AgendaItemSections;
using MMA.WebApi.Shared.Interfaces.AgendaItemVoting;
using MMA.WebApi.Shared.Interfaces.SectionBidderList;
using MMA.WebApi.Shared.Interfaces.SectionBudget;
using MMA.WebApi.Shared.Models;
using MMA.WebApi.Shared.Models.AgendaItemSections;
using MMA.WebApi.Shared.Models.AgendaItemVoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.Forms.Domain.Services
{
    public class AgendaItemFormsService : IAgendaItemFormsService
    {
        private readonly ISectionBidderListRepository sectionBidderListRepository;
        private readonly ISectionBudgetRepository sectionBudgetRepository;
        private readonly IAgendaItemRepository agendaItemRepository;
        private readonly ISectionRepository sectionRepository;
        private readonly IAgendaItemSectionRepository agendaItemSectionRepository;
        private readonly ISectionSubjectRepository sectionSubjectRepository;
        private readonly ISectionIntroductionRepository sectionIntroductionRepository;
        private readonly ISectionTenderStrategyRepository sectionTenderStrategyRepository;
        private readonly ISectionClarificationRepository sectionClarificationRepository;
        private readonly ISectionRecommendationRepository sectionRecommendationRepository;
        private readonly ISectionGeneralInfoRepository sectionGeneralInfoRepository;

        public AgendaItemFormsService(
                ISectionBidderListRepository sectionBidderListRepository,
                ISectionBudgetRepository sectionBudgetRepository,
                IAgendaItemRepository agendaItemRepository,
                ISectionRepository sectionRepository, 
                ISectionSubjectRepository sectionSubjectRepository,
                ISectionIntroductionRepository sectionIntroductionRepository,
                ISectionTenderStrategyRepository sectionTenderStrategyRepository,
                ISectionClarificationRepository sectionClarificationRepository,
                ISectionRecommendationRepository sectionRecommendationRepository,
                ISectionGeneralInfoRepository sectionGeneralInfoRepository,
                IAgendaItemSectionRepository agendaItemSectionRepository
            )
        {
            this.sectionBidderListRepository = sectionBidderListRepository;
            this.sectionBudgetRepository = sectionBudgetRepository;
            this.agendaItemRepository = agendaItemRepository;
            this.sectionRepository = sectionRepository;
            this.sectionSubjectRepository = sectionSubjectRepository;
            this.sectionIntroductionRepository = sectionIntroductionRepository;
            this.sectionTenderStrategyRepository = sectionTenderStrategyRepository;
            this.sectionClarificationRepository = sectionClarificationRepository;
            this.sectionRecommendationRepository = sectionRecommendationRepository;
            this.sectionGeneralInfoRepository = sectionGeneralInfoRepository;
            this.agendaItemSectionRepository = agendaItemSectionRepository;
        }

        public async Task<List<SectionModel>> GetSections(int agendaItemId)
        {
            List<SectionModel> sections = new List<SectionModel>();

            var agendaItem = agendaItemRepository.Get().Where(x => x.Id == agendaItemId).FirstOrDefault();

            if (agendaItem != null)
            {
                // todo get agenda item company id>
                sections = agendaItemSectionRepository.GetSections(agendaItemId).Result;
            }

            return sections;
        }

        public async Task<AgendaItemBaseSectionModel> Get(int agendaItemId, AgendaItemSectionsList sectionId)
        {
            var repository = GetSectionRepository(sectionId);
            
            var data= repository.Get(agendaItemId).Result;

            // set sectionInstanceId
            var sectionsInstances = agendaItemSectionRepository.GetSections(agendaItemId).Result;
            var sectionInstance = sectionsInstances.Where(x => x.SectionId == sectionId).FirstOrDefault();

            if (sectionInstance != null)
                data.SectionInstanceId = sectionInstance.Id;
            data.AgendaItemId = agendaItemId;

            return data;
        }

        public async Task Save(AgendaItemBaseSectionModel model, string userId)
        {
            var repository = GetSectionRepository(model.sectionId);
            SectionValidation(model);
            await repository.Save(model, userId);
        }

        private void SectionValidation(AgendaItemBaseSectionModel model)
        {
            bool IsValid = true;
            StringBuilder sbErrorMessage = new StringBuilder();

            switch (model.sectionId)
            {
                case AgendaItemSectionsList.GENERAL_INFO:
                    break;
                case AgendaItemSectionsList.SUBJECT:
                    break;
                case AgendaItemSectionsList.BUDGET:

                    SectionBudgetModel modelBudget = model as SectionBudgetModel;

                    if (!modelBudget.EstimatedValueUSD.HasValue || modelBudget.EstimatedValueUSD == 0)
                    {
                        IsValid = false;
                        sbErrorMessage.AppendLine("Budget USD is required.");
                    }
                    if (!modelBudget.EstimatedValueAED.HasValue || modelBudget.EstimatedValueAED == 0)
                    {
                        IsValid = false;
                        sbErrorMessage.AppendLine("Budget AED is required.");
                    }
                    if (!modelBudget.BudgetTypeCapex.HasValue)
                    {
                        IsValid = false;
                        sbErrorMessage.AppendLine("CAPEX is required.");
                    }
                    if (!modelBudget.BudgetTypeOpex.HasValue)
                    {
                        IsValid = false;
                        sbErrorMessage.AppendLine("OPEX is required.");
                      
                    }

                    break;
                case AgendaItemSectionsList.BIDDER_LIST_BL:
                    break;
                case AgendaItemSectionsList.BIDDER_LIST_TE:
                    break;
                case AgendaItemSectionsList.BIDDER_LIST_AW:
                    break;
                case AgendaItemSectionsList.INTODUCTIOIN_BACKGROUND:
                    break;
                case AgendaItemSectionsList.TENDER_STRATEGY:
                    break;
                case AgendaItemSectionsList.CLARIFICATION:
                    break;
                case AgendaItemSectionsList.RECOMMENDATION:
                    break;
                case AgendaItemSectionsList.BIDDER_LIST_TCE:
                    break;
                default:
                    break;
            }

            if (!IsValid)
            {
                throw new Exception(sbErrorMessage.ToString());
            }

        }

        /// <summary>
        /// return section repository based on input param
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        private ISectionBaseRepository GetSectionRepository(AgendaItemSectionsList sectionId)
        {
            switch (sectionId)
            {
                case AgendaItemSectionsList.GENERAL_INFO:
                    return sectionGeneralInfoRepository;


                case AgendaItemSectionsList.SUBJECT:
                    return sectionSubjectRepository;

                case AgendaItemSectionsList.INTODUCTIOIN_BACKGROUND:
                    return sectionIntroductionRepository;

                case AgendaItemSectionsList.TENDER_STRATEGY:
                    return sectionTenderStrategyRepository;

                case AgendaItemSectionsList.CLARIFICATION:
                    return sectionClarificationRepository;

                case AgendaItemSectionsList.RECOMMENDATION:
                    return sectionRecommendationRepository;

                case AgendaItemSectionsList.BUDGET:
                    return sectionBudgetRepository;

                case AgendaItemSectionsList.BIDDER_LIST_BL:
                case AgendaItemSectionsList.BIDDER_LIST_TE:
                case AgendaItemSectionsList.BIDDER_LIST_AW:
                case AgendaItemSectionsList.BIDDER_LIST_TCE:

                    return sectionBidderListRepository;
            }

            throw new Exception($"Wrong value for sectionId: {sectionId}");

        }


        public async Task CreateSectionInstances(int agendaItemId, string userId)
        {            
            var agendaItem = agendaItemRepository.Get().Where(x => x.Id == agendaItemId).FirstOrDefault();

            // get sections for specific type and comapny
            var sections = sectionRepository.GetFormSectionsList(agendaItem.TypeId, agendaItem.CompanyId).Result;

            foreach (SectionModel section in sections)
            {
                // create instances

                await agendaItemSectionRepository.CreateSectionInstance(agendaItemId, section, userId);
            }

        }


    }
}
