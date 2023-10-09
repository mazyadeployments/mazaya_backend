using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.AdnocTermsAndConditions;
using MMA.WebApi.Shared.Models.AdnocTermsAndConditions;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Declares;

namespace MMA.WebApi.DataAccess.Repositories.AdnocTermsAndConditions
{
    public class AdnocTermsAndConditionsRepository : BaseRepository<AdnocTermsAndConditionsModel>, IAdnocTermsAndConditionsRepository
    {
        public AdnocTermsAndConditionsRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public async Task<AdnocTermsAndConditionsModel> GetAdnocTermsAndConditions(AdnocTermsAndConditionType type)
        {
            var context = ContextFactory();

            return await (from atc in context.AdnocTermsAndConditions
                          where atc.Type == type
                          orderby atc.CreatedOn descending
                          select new AdnocTermsAndConditionsModel
                          {
                              Id = atc.Id,
                              Content = atc.Content,
                              ContentArabic = atc.ContentArabic,
                              Type = atc.Type,
                          }).FirstOrDefaultAsync();
        }

        public async Task UpdateAdnocTermsAndConditions(AdnocTermsAndConditionsModel adnocTermsAndConditionsModel, string userId)
        {
            var context = ContextFactory();

            context.AdnocTermsAndConditions.Add(new Models.AdnocTermsAndConditions()
            {
                Content = adnocTermsAndConditionsModel.Content,
                ContentArabic = adnocTermsAndConditionsModel.ContentArabic,
                Type = adnocTermsAndConditionsModel.Type,
                CreatedBy = userId,
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = userId,
                UpdatedOn = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        protected override IQueryable<AdnocTermsAndConditionsModel> GetEntities()
        {
            throw new NotImplementedException();
        }
    }
}
