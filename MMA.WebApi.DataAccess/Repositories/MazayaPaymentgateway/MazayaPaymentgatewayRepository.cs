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
using PdfSharp.Pdf;
using static IdentityServer4.Models.IdentityResources;
using MMA.WebApi.Shared.Models.MazayaEcarddetail;

namespace MMA.WebApi.DataAccess.Repositories.MazayaPaymentgateway
{
    public class MazayaPaymentgatewayRepository : BaseRepository<MazayaPaymentgatewayModel>, IMazayaPaymentgatewayRepository
    {
        public MazayaPaymentgatewayRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }
        public async Task<MazayaPaymentgatewayModel> CreateOrUpdateAsync(MazayaPaymentgatewayModel model, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();

            var mazayapaymentgateway = context.MazayaPaymentgateways.FirstOrDefault(x => x.Id == model.Id);
            if (mazayapaymentgateway == null)
                mazayapaymentgateway = new Models.MazayaPaymentgateway();

            PopulateEntityModel(mazayapaymentgateway, model);

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
            response_code = data.response_code,
            card_number = data.card_number,
            card_holder_name = data.card_holder_name,
            payment_option = data.payment_option,
            expiry_date = data.expiry_date,
            customer_ip = data.customer_ip,
            eci = data.eci,
            fort_id = data.fort_id,
            response_msg = data.response_msg,
            authorization_code = data.authorization_code,
            merchant_reference = data.merchant_reference,
            cust_email = data.cust_email,
            Bankref = data.Bankref,
            Device = data.Device,
            Deviceid = data.Deviceid,
            Cardname = data.Cardname,
            Cardno = data.Cardno,
            PayDate = data.PayDate,
            Name = data.Name,
            Address = data.Address,
            Amount = data.Amount,
            Paystatus = data.Paystatus,
            CreatedOn = data.CreatedOn,
            CreatedBy = data.CreatedBy,
            UpdatedOn = data.UpdatedOn,
            UpdatedBy = data.UpdatedBy 
        };

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaPaymentgateway, MazayaPaymentgatewayModel>> projectTopaymentgatewayCardModel = data =>
           new MazayaPaymentgatewayModel()
           {
               Id = data.Id,
               response_code = data.response_code,
               card_number = data.card_number,
               card_holder_name = data.card_holder_name,
               payment_option = data.payment_option,
               expiry_date = data.expiry_date,
               customer_ip = data.customer_ip,
               eci = data.eci,
               fort_id = data.fort_id,
               response_msg = data.response_msg,
               authorization_code = data.authorization_code,
               merchant_reference = data.merchant_reference,
               cust_email = data.cust_email,
               Bankref = data.Bankref,
               Device = data.Device,
               Deviceid = data.Deviceid,
               Cardname = data.Cardname,
               Cardno = data.Cardno,
               PayDate = data.PayDate,
               Name = data.Name,
               Address = data.Address,
               Amount = data.Amount,
               Paystatus = data.Paystatus,
               CreatedOn = data.CreatedOn,
               CreatedBy = data.CreatedBy,
               UpdatedOn = data.UpdatedOn,
               UpdatedBy = data.UpdatedBy
           };

        private void PopulateEntityModel(MMA.WebApi.DataAccess.Models.MazayaPaymentgateway data, MazayaPaymentgatewayModel model)
        {
            data.Id = model.Id;
            data.response_code = model.response_code;
            data.card_number = model.card_number;
            data.card_holder_name = model.card_holder_name;
            data.payment_option = model.payment_option;
            data.expiry_date = model.expiry_date;
            data.customer_ip = model.customer_ip;
            data.eci = model.eci;
            data.fort_id = model.fort_id;
            data.response_msg = model.response_msg;
            data.authorization_code = model.authorization_code;
            data.merchant_reference = model.merchant_reference;
            data.cust_email = model.cust_email;
            data.Bankref = model.Bankref;
            data.Device = model.Device;
            data.Deviceid = model.Deviceid;
            data.Cardname = model.Cardname;
            data.Cardno = model.Cardno;
            data.PayDate = model.PayDate;
            data.Name = model.Name;
            data.Address = model.Address;
            data.Amount = model.Amount;
            data.Paystatus = model.Paystatus;
            data.CreatedOn = model.CreatedOn;
            data.CreatedBy = model.CreatedBy;
            data.UpdatedOn = model.UpdatedOn;
            data.UpdatedBy = model.UpdatedBy;
        }
    }
}
