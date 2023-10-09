using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Document;
using MMA.WebApi.Shared.Models.Document;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess
{
    public class DocumentRepository : BaseRepository<DocumentFileModel>, IDocumentRepository
    {
        private readonly IConfiguration _configuration;

        public DocumentRepository(Func<MMADbContext> contexFactory, IConfiguration configuration)
            : base(contexFactory)
        {
            _configuration = configuration;
        }

        public IQueryable<DocumentFileModel> Get()
        {
            return GetEntities();
        }

        public async Task DeleteAsync(string id)
        {
            new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            var context = ContextFactory();
            var document = context.Document.FirstOrDefault(x => x.Id.Equals(id));

            if (document != null)
            {
                context.Remove(document);
                context.SaveChanges();
            }
        }

        public async Task<string> EditAsync(DocumentFileModel data)
        {
            var context = ContextFactory();
            var entityModel = context.Document.FirstOrDefault(x => x.Id.Equals(data.Id));
            entityModel = PopulateDbFromDomainModel(entityModel, data);
            entityModel.Id = data.Id;

            ChangeableHelper.Set(entityModel, "sysadmin", false);
            context.SaveChanges();

            return entityModel.Id.ToString();
        }

        public async Task<IEnumerable<DocumentFileModel>> GetListAsync(
            Expression<Func<DocumentFileModel, bool>> query = null
        )
        {
            if (query != null)
            {
                return await GetEntities().Where(query).ToListAsync();
            }
            return await GetEntities().ToListAsync();
        }

        public async Task<DocumentFileModel> GetSingleAsync(
            Expression<Func<DocumentFileModel, bool>> query
        )
        {
            return await GetEntities().FirstOrDefaultAsync(query);
        }

        public async Task<string> InsertAsync(DocumentFileModel data)
        {
            var context = ContextFactory();
            var entitModel = PopulateDbFromDomainModel(new Document(), data);
            entitModel.Id = data.Id;
            ChangeableHelper.Set(entitModel, "sysadmin", true);
            await context.Document.AddAsync(entitModel);

            await context.SaveChangesAsync();

            return entitModel.Id.ToString();
        }

        protected override IQueryable<DocumentFileModel> GetEntities()
        {
            var context = ContextFactory();

            return from p in context.Document
                select new DocumentFileModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Content = p.Content,
                    MimeType = p.MimeType,
                    Size = p.Size,
                    StorageType = p.StorageType,
                    UploadedOn = p.UpdatedOn,
                    StoragePath = p.StoragePath,
                    ParentId = p.ParentId
                };
        }

        protected virtual Document PopulateDbFromDomainModel(
            Document entityModel,
            DocumentFileModel data
        )
        {
            //entityModel.Id = data.Id;
            entityModel.Name = data.Name;
            entityModel.Content = data.Content;
            entityModel.MimeType = data.MimeType;
            entityModel.Size = data.Size;
            entityModel.StorageType = data.StorageType;
            entityModel.StoragePath = data.StoragePath;
            entityModel.ParentId = data.ParentId;

            return entityModel;
        }

        public Task DeleteListAsync(IEnumerable<string> list)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteRedundantImages()
        {
            var context = ContextFactory();

            using (
                IDbConnection db = new SqlConnection(
                    this._configuration.GetConnectionString("eMarketOffers")
                )
            )
            {
                bool flag = true;
                while (flag)
                {
                    string sql =
                        "select Top(500)* from Document  "
                        + "where id not in (select id from document where  id in "
                        + "("
                        + "        (select DocumentId from ApplicationUserDocument) union"
                        + "        (select DocumentId from OfferImages) union"
                        + "        (select DocumentId from CategoryDocument) union"
                        + "        (select DocumentId from CollectionDocument) union"
                        + "        (select DocumentId from CompanyDocument) union"
                        + "        (select SpecialAnnouncement from Offer) union"
                        + "        (select DocumentId from OfferDocument) union"
                        + "        (select DocumentId from RoadshowDocument) union"
                        + "        (select DocumentId from RoadshowOfferDocument) union"
                        + "        (select DocumentId from RoadshowOfferProposalDocument) union"
                        + "        (select DocumentIdHorizontalBackPicture from MembershipPictureDatas) union"
                        + "        (select DocumentIdHorizontalPicture from MembershipPictureDatas) union"
                        + "        (select DocumentIdVerticalBackPicture from MembershipPictureDatas) union"
                        + "        (select DocumentIdVerticalPicture from MembershipPictureDatas) union"
                        + "        (select TradeLicenceId from Company) union"
                        + "        (select EmiratesId from Roadshow) union"
                        + "        (select DocumentId from AnnouncementAttachments) union"
                        + "        (select DocumentId from MailStorageDocument) union"
                        + "        (select PhotoUrl from MembershipECards)"
                        + ")"
                        + ") ";
                    var result = db.Query<Document>(sql);

                    flag = result.Count() > 0;
                    if (!flag)
                        continue;

                    context.Document.RemoveRange(result);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
