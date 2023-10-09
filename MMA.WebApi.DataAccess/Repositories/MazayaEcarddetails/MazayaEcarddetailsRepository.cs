using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.MazayaEcardetails;
using MMA.WebApi.Shared.Interfaces.MazayaEcardmain;
using MMA.WebApi.Shared.Models.MazayaEcarddetail;
using MMA.WebApi.Shared.Models.MazayaEcardmain;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Repositories.MazayaEcarddetails
{
    public class MazayaEcarddetailsRepository : BaseRepository<MazayaEcarddetailsModel>, IMazayaEcarddetailsRepository
    {
        public MazayaEcarddetailsRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

        public async Task<MazayaEcarddetailsModel> CreateOrUpdateAsync(MazayaEcarddetailsModel model, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();

            var mazayaecarddetails = context.MazayaEcarddetails.FirstOrDefault(x => x.id == model.id);
            if (mazayaecarddetails == null)
                mazayaecarddetails = new Models.MazayaEcarddetails();

            PopulateEntityModel(mazayaecarddetails, model);


            if (model.id == 0)
            {
                mazayaecarddetails.status = "New";
                mazayaecarddetails.Accept(auditVisitor);
                context.Add(mazayaecarddetails);
            }
            else
            {

                mazayaecarddetails.UpdatedOn = DateTime.UtcNow;
                context.Update(mazayaecarddetails);
            }

            await context.SaveChangesAsync();

            return projectToMazayaEcarddetailsModel.Compile().Invoke(mazayaecarddetails);
        }

        public async Task<MazayaEcarddetailsModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var mazayacategory = await context.MazayaEcarddetails
                        .AsNoTracking()
                        .Select(projectToMazayaEcarddetailsModel)
                        .FirstOrDefaultAsync(x => x.id == id);


            return mazayacategory;
        }

        public async Task<MazayaEcarddetailsModel> Get(int id)
        {
            var context = ContextFactory();

            return await context.MazayaEcarddetails
                    .AsNoTracking()
                    .Select(projectToMazayaEcarddetailsModel)
                    .FirstOrDefaultAsync(x => x.id == id);
        }

        public IQueryable<MazayaEcarddetailsModel> GetAllEcarddetails()
        {
            var context = ContextFactory();

            return context.MazayaEcarddetails
                .Select(projectToMazayaEcarddetailsModel);
        }

        public async Task<int> GetMazayaEcarddetailsCount()
        {
            var context = ContextFactory();

            return await context.MazayaEcarddetails.CountAsync();
        }

        public IEnumerable<MazayaEcarddetailsModel> GetMazayaEcarddetailsNumber()
        {
            var context = ContextFactory();

            var categories = context.MazayaEcarddetails
                .AsNoTracking()
                .Select(projectToMazayaEcarddetailsModel);

            return categories.OrderBy(c => c.id);
        }

        public IQueryable<MazayaEcarddetailsModel> GetMazayaEcarddetailsNumberPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            var mazayaecardmains = context.MazayaEcarddetails.AsNoTracking();
            var categoryModels = mazayaecardmains.Select(projectToMazayaEcarddetailsModel);

            return Sort(queryModel.Sort, categoryModels);
        }

        private static IQueryable<MazayaEcarddetailsModel> Sort(SortModel sortModel, IQueryable<MazayaEcarddetailsModel> mazayaecarddetails)
        {
            // Currently sorting needs to be done alphabetically
            return mazayaecarddetails.OrderBy(c => c.id);
        }

        private void PopulateEntityModel(MMA.WebApi.DataAccess.Models.MazayaEcarddetails data, MazayaEcarddetailsModel model)
        {
            data.id = model.id;
            data.firstname = model.firstname;
            data.lastname = model.lastname;
            data.relation = model.relation;
            data.card_number = model.card_number;
            data.MazayaEcardmainid = model.MazayaEcardmainId;
            data.status = model.status;
            //data.dob = DateTime.Parse(model.dob);
            if (string.IsNullOrEmpty(model.dob))
            {
                data.dob = DateTime.MinValue;
            }
            else
            {
                data.dob = DateTime.Parse(model.dob);
            }
            data.CreatedBy = model.CreatedBy;
            data.CreatedOn = model.CreatedOn;
            data.UpdatedBy = model.UpdatedBy;
            data.UpdatedOn = model.UpdatedOn;
        }

        protected override IQueryable<MazayaEcarddetailsModel> GetEntities()
        {
            throw new NotImplementedException();
        }

        private Expression<Func<MMA.WebApi.DataAccess.Models.MazayaEcarddetails, MazayaEcarddetailsModel>> projectToMazayaEcarddetailsModel = data =>
       new MazayaEcarddetailsModel()
       {
           id = data.id,
           firstname = data.firstname,
           lastname = data.lastname,
           relation = data.relation,
           card_number = data.card_number,
           status = data.status,
           MazayaEcardmainId = data.MazayaEcardmainid,
           CreatedBy = data.CreatedBy,
           CreatedOn = data.CreatedOn,
           UpdatedBy = data.UpdatedBy,
           UpdatedOn = data.UpdatedOn,

       };
    }
}
