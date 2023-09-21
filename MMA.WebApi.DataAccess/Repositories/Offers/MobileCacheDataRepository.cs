using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Interfaces.Offers;
using MMA.WebApi.Shared.Models.Mobile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.Offers
{
    public class MobileCacheDataRepository : BaseRepository<MobileCacheData>, IMobileCacheDataRepository
    {
        public MobileCacheDataRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public async Task<string> GetSynchronizationDataModel()
        {
            return GetDataEntities();
        }

        public async Task SetMobileCacheData(SynchronizationDataModel model)
        {
            var context = ContextFactory();
            var mobileData = await context.MobileCacheDatas.FirstOrDefaultAsync();
            if (mobileData != null)
            {
                mobileData.Data = JsonConvert.SerializeObject(model);
                context.MobileCacheDatas.Update(mobileData);
            }
            else
            {
                var tasks = new List<Task>();
                tasks.Add(Task.Run(() => AddMobileData(model, context)));
                await Task.WhenAll(tasks);
            }
            await context.SaveChangesAsync();
        }

        private static void AddMobileData(SynchronizationDataModel model, MMADbContext context)
        {
            var newMobileData = new MobileCacheData();
            newMobileData.Data = JsonConvert.SerializeObject(model);
            context.MobileCacheDatas.Add(newMobileData);
        }

        protected override IQueryable<MobileCacheData> GetEntities()
        {
            var context = ContextFactory();

            return context.MobileCacheDatas.Select(x => new MobileCacheData { Data = x.Data });
        }

        protected string GetDataEntities()
        {
            var context = ContextFactory();
            var mobileCache = context.MobileCacheDatas.FirstOrDefault();
            if (mobileCache != null) return mobileCache.Data;

            return null;
        }
    }
}
