using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.MazayaCategory;
using MMA.WebApi.Shared.Interfaces.MazayaEcardmain;
using MMA.WebApi.Shared.Models.MazayaCategory;
using MMA.WebApi.Shared.Models.MazayacategoryDocument;
using MMA.WebApi.Shared.Models.MazayaEcardmain;
using MMA.WebApi.Shared.Visitor;
using PdfSharp.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MMA.WebApi.Shared.Enums.Document.DocumentDeclares;

namespace MMA.WebApi.DataAccess.Repositories.MazayaEcardmain
{
    public class MazayaEcardmainRepository : BaseRepository<MazayaEcardmainModel>, IMazayaEcardmainRepository
    {
        public MazayaEcardmainRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        private int _currentInvoiceNumber;
        public async Task<MazayaEcardmainModel> CreateOrUpdateAsync(MazayaEcardmainModel model, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();

            var mazayaecardmain = context.MazayaEcardmains.FirstOrDefault(x => x.id == model.id);
            if (mazayaecardmain == null)
                mazayaecardmain = new Models.MazayaEcardmain();

            PopulateEntityModel(mazayaecardmain, model);

           
            if (model.id == 0)
            {
                mazayaecardmain.currency = "Dhs";
                mazayaecardmain.status = "New";
                mazayaecardmain.Accept(auditVisitor);
                context.Add(mazayaecardmain);
            }
            else
            {
                var maxValue = context.MazayaEcardmains.Max(item => item.invoice_number);
                if (maxValue == "")
                {
                    maxValue = "1";
                    string formattedNumber = maxValue.ToString().PadLeft(5, '0');
                    mazayaecardmain.invoice_number = "INV-" + formattedNumber;
                }
                else
                {
                    string formattedNumber = maxValue.ToString().PadLeft(5, '0');
                    string digitsOnly = Regex.Replace(formattedNumber, "[^0-9]", "");
                    int val = int.Parse(digitsOnly);
                    val++;
                    string num = val.ToString().PadLeft(5, '0');
                    mazayaecardmain.invoice_number = "INV-" + num;
                    
                }
               
                mazayaecardmain.status = "Paid";
                mazayaecardmain.date = DateTime.Now.ToString();
                mazayaecardmain.UpdatedOn = DateTime.UtcNow;
                context.Update(mazayaecardmain);
            }

            await context.SaveChangesAsync();

            return projectToMazayaEcardmainModel.Compile().Invoke(mazayaecardmain);
        }

        public async Task<MazayaEcardmainModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var mazayacategory = await context.MazayaEcardmains
                        .AsNoTracking()
                        .Select(projectToMazayaEcardmainModel)
                        .FirstOrDefaultAsync(x => x.id == id);

            
            return mazayacategory;
        }

        public async Task<MazayaEcardmainModel> Get(int id)
        {
            var context = ContextFactory();

            return await context.MazayaEcardmains
                    .AsNoTracking()
                    .Select(projectToMazayaEcardmainModel)
                    .FirstOrDefaultAsync(x => x.id == id);
        }

        public IQueryable<MazayaEcardmainModel> GetAllEcardmain()
        {
            var context = ContextFactory();

            return context.MazayaEcardmains
                .Select(projectToMazayaEcardmainModel);
        }

        public async Task<int> GetMazayaEcardmainCount()
        {
            var context = ContextFactory();

            return await context.MazayaEcardmains.CountAsync();
        }

        public IEnumerable<MazayaEcardmainModel> GetMazayaEcardmainNumber()
        {
            var context = ContextFactory();

            var categories = context.MazayaEcardmains
                .AsNoTracking()
                .Select(projectToMazayaEcardmainModel);

            return categories.OrderBy(c => c.id);
        }

        public IQueryable<MazayaEcardmainModel> GetMazayaEcardmainNumberPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            var mazayaecardmains = context.MazayaEcardmains.AsNoTracking();
            var categoryModels = mazayaecardmains.Select(projectToMazayaEcardmainModel);

            return Sort(queryModel.Sort, categoryModels);
        }

        protected override IQueryable<MazayaEcardmainModel> GetEntities()
        {
            var context = ContextFactory();

            return context.MazayaEcardmains
                .Select(projectToMazayaEcardmainModel);
        }

        private static IQueryable<MazayaEcardmainModel> Sort(SortModel sortModel, IQueryable<MazayaEcardmainModel> mazayaecardmain)
        {
            // Currently sorting needs to be done alphabetically
            return mazayaecardmain.OrderBy(c => c.id);
        }

        private void PopulateEntityModel(MMA.WebApi.DataAccess.Models.MazayaEcardmain data, MazayaEcardmainModel model)
        {
            model.id = data.id;
            model.invoice_number = data.invoice_number;
            model.date = data.date;
            model.date_expire = data.date_expire;
            model.amount = data.amount;
            model.vat = data.vat;
            model.grandtotal= data.grandtotal;
            model.status= data.status;
            model.subcategoryids = data.subcategoryids;
            model.CreatedBy = data.CreatedBy;
            model.CreatedOn = data.CreatedOn;
            model.UpdatedBy = data.UpdatedBy;
            model.UpdatedOn = data.UpdatedOn;
        }

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaEcardmain, MazayaEcardmainModel>> projectToMazayaEcardmainModel = data =>
        new MazayaEcardmainModel()
        {
            id = data.id,
            invoice_number = data.invoice_number,
            date = data.date,
            date_expire = data.date_expire,
            amount = data.amount,
            vat = data.vat,
            grandtotal = data.grandtotal,
            status = data.status,
            subcategoryids = data.subcategoryids,
            CreatedBy = data.CreatedBy,
            CreatedOn = data.CreatedOn,
            UpdatedBy = data.UpdatedBy,
            UpdatedOn = data.UpdatedOn,

        };


    }
}
