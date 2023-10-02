using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.MazayaPaymentgateway;
using MMA.WebApi.Shared.Models.MazayaPaymentgateway;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.MazayaPaymentgateway
{
    public class MazayaPaymentgatewayRepository : BaseRepository<MazayaPaymentgatewayModel>, IMazayaPaymentgatewayRepository
    {
        public MazayaPaymentgatewayRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }
        public async Task<MazayaPaymentgatewayModel> CreateOrUpdateAsync(MazayaPaymentgatewayModel model, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();

            var mazayapaymentgateway = context.MazayaPaymentgateways.FirstOrDefault(x => x.Id == model.Id);

            if (model.Id == 0)
            {
                mazayapaymentgateway.Accept(auditVisitor);
                context.Add(mazayapaymentgateway);
            }
            else
            {
                mazayapaymentgateway.UpdatedOn = DateTime.UtcNow;
                context.Update(mazayapaymentgateway);
            }

            await context.SaveChangesAsync();

            return projectToMazayapaymentgatewayModel.Compile().Invoke(mazayapaymentgateway);
        }

        public async Task<MazayaPaymentgatewayModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var mazayapaymentgateway = await context.MazayaPaymentgateways
                        .AsNoTracking()
                        .Select(projectToMazayapaymentgatewayModel)
                        .FirstOrDefaultAsync(x => x.Id == id);

            if (mazayapaymentgateway != null)
            {
                var data = new MMA.WebApi.DataAccess.Models.MazayaPaymentgateway();
                data.Id = mazayapaymentgateway.Id;

                context.Remove(data);
                context.SaveChanges();
            }
            return mazayapaymentgateway;
        }

        public async Task<MazayaPaymentgatewayModel> Get(int id)
        {
            var context = ContextFactory();

            return await context.MazayaPaymentgateways
                    .AsNoTracking()
                    .Select(projectToMazayapaymentgatewayModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<MazayaPaymentgatewayModel> GetAllPaymentgateway()
        {
            var context = ContextFactory();

            var mazayapaymentgateway = context.MazayaPaymentgateways
                               .Select(projectToMazayapaymentgatewayModel);
            return mazayapaymentgateway.OrderBy(c => c.Id);
        }

        public async Task<int> GetMazayaPaymentgatewayCount()
        {
            var context = ContextFactory();

            return await context.MazayaPaymentgateways.CountAsync();
        }

        public IEnumerable<MazayaPaymentgatewayModel> GetMazayaPaymentgatewayNumber()
        {
            var context = ContextFactory();

            var mazayapaymentgateway = context.MazayaPaymentgateways
                .AsNoTracking()
                .Select(projectToMazayapaymentgatewayModel);

            return mazayapaymentgateway.OrderBy(c => c.Id);
        }

        public IQueryable<MazayaPaymentgatewayModel> GetMazayaPaymentgatewayNumberPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            var mazayapaymentgateway = context.MazayaPaymentgateways.AsNoTracking();
            var paymentModels = mazayapaymentgateway.Select(projectToMazayapaymentgatewayModel);

            return Sort(queryModel.Sort, paymentModels);
        }

        protected override IQueryable<MazayaPaymentgatewayModel> GetEntities()
        {
            var context = ContextFactory();

            return context.MazayaPaymentgateways
                .Select(projectToMazayapaymentgatewayModel);
        }

        private static IQueryable<MazayaPaymentgatewayModel> Sort(SortModel sortModel, IQueryable<MazayaPaymentgatewayModel> mazayapaymentcategories)
        {
            // Currently sorting needs to be done alphabetically
            return mazayapaymentcategories.OrderBy(c => c.Id);
        }

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaPaymentgateway, MazayaPaymentgatewayModel>> projectToMazayapaymentgatewayModel = data =>
        new MazayaPaymentgatewayModel()
        {
            Id = data.Id,
            Name = data.Name,
            Bankref = data.Bankref,
            Device = data.Device,
            Deviceid = data.Deviceid,
            Cardname = data.Cardname,
            Cardtype = data.Cardtype,
            Cardno = data.Cardno,
            PayDate = data.PayDate,
            Address = data.Address,
            Amount = data.Amount,
            Paystatus = data.Paystatus,
            CreatedBy = data.CreatedBy,
            CreatedOn = data.CreatedOn,
            UpdatedBy = data.UpdatedBy,
            UpdatedOn = data.UpdatedOn
        };

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaPaymentgateway, MazayaPaymentgatewayModel>> projectTopaymentgatewayCardModel = data =>
           new MazayaPaymentgatewayModel()
           {
               Id = data.Id,
               Name = data.Name,
               Bankref = data.Bankref,
               Device = data.Device,
               Deviceid = data.Deviceid,
               Cardname = data.Cardname,
               Cardtype = data.Cardtype,
               Cardno = data.Cardno,
               PayDate = data.PayDate,
               Address = data.Address,
               Amount = data.Amount,
               Paystatus = data.Paystatus,
               UpdatedOn = data.UpdatedOn,
               UpdatedBy = data.UpdatedBy,
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn
           };
    }
}
