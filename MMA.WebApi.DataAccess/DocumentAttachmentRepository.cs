//using Microsoft.EntityFrameworkCore;
//using MMA.WebApi.DataAccess.Models;
//using MMA.WebApi.Shared.Helpers;
//using MMA.WebApi.Shared.Interfaces.Document;
//using MMA.WebApi.Shared.Models.Document;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;

//namespace MMA.WebApi.DataAccess
//{
//    public class DocumentAttachmentRepository : BaseRepository<DocumentAttachment>, IDocumentAttachmentRepository

//    {
//        public DocumentAttachmentRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }

//        public IQueryable<DocumentAttachment> Get()
//        {
//            return GetEntities();
//        }

//        public async Task DeleteAsync(string id)
//        {
//            new NotImplementedException();
//        }

//        public async Task<string> EditAsync(DocumentAttachment data)
//        {
//            var context = ContextFactory();
//            var entityModel = context.AgendaItemAttachment.FirstOrDefault(x => x.AgendaItemId.Equals(data.AgendaItemId));
//            entityModel = PopulateDbFromDomainModel(entityModel, data);
//            entityModel.AgendaItemId = data.AgendaItemId;

//            ChangeableHelper.Set(entityModel, "sysadmin", false);
//            context.SaveChanges();

//            return entityModel.AgendaItemId.ToString();
//        }

//        public async Task<IEnumerable<DocumentAttachment>> GetListAsync(Expression<Func<DocumentAttachment, bool>> query = null)
//        {
//            if (query != null)
//            {
//                return await GetEntities().Where(query).ToListAsync();
//            }
//            return await GetEntities().ToListAsync();
//        }

//        public async Task<DocumentAttachment> GetSingleAsync(Expression<Func<DocumentAttachment, bool>> query)
//        {
//            return await GetEntities().FirstOrDefaultAsync(query);
//        }

//        public async Task<string> InsertAsync(DocumentAttachment data)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<string> InsertAsync(DocumentAttachment data, string userId)
//        {
//            var context = ContextFactory();
//            var entitModel = PopulateDbFromDomainModel(new AgendaItemAttachment(), data);
//            entitModel.AgendaItemId = data.AgendaItemId;
//            ChangeableHelper.Set(entitModel, userId, true);
//            await context.AgendaItemAttachment.AddAsync(entitModel);

//            await context.SaveChangesAsync();

//            return entitModel.AgendaItemId.ToString();
//        }

//        protected override IQueryable<DocumentAttachment> GetEntities()
//        {
//            var context = ContextFactory();

//            return from p in context.AgendaItemAttachment
//                   select new DocumentAttachment
//                   {
//                       AgendaItemId = p.AgendaItemId,
//                       AttachmentId = p.DocumentId,
//                       //Content = p.Content,
//                       //MimeType = p.MimeType,
//                       //Size = p.Size,
//                       //StorageType = p.StorageType,
//                       //UploadedOn = p.UpdatedOn,
//                       //StoragePath = p.StoragePath,
//                   };
//        }

//        protected virtual AgendaItemAttachment PopulateDbFromDomainModel(AgendaItemAttachment entityModel, DocumentAttachment data)
//        {
//            //entityModel.Id = data.Id;
//            entityModel.AgendaItemId = data.AgendaItemId;
//            entityModel.DocumentId = data.AttachmentId;
//            entityModel.AgendaItemDocumentId = data.AgendaItemDocumentId;
//            entityModel.AgendaItemSectionId = data.AgendaItemSectionId;
//            entityModel.AttachmentType = data.AttachmentType;

//            //entityModel.MimeType = data.MimeType;
//            //entityModel.Size = data.Size;
//            //entityModel.StorageType = data.StorageType;
//            //entityModel.StoragePath = data.StoragePath;

//            return entityModel;
//        }

//        public Task DeleteListAsync(IEnumerable<string> list)
//        {
//            throw new NotImplementedException();
//        }

//        #region Agenda Item Action Attachments
//        public async Task<string> InsertActionAttachment(DocumentActionAttachment data)
//        {
//            var context = ContextFactory();
//            var entitModel = PopulateDbFromDomainModel(new AgendaItemActionAttachment(), data);
//            entitModel.AgendaItemActionId = data.AgendaItemActionId;
//            ChangeableHelper.Set(entitModel, "sysadmin", true);
//            await context.AgendaItemActionAttachment.AddAsync(entitModel);

//            await context.SaveChangesAsync();

//            return entitModel.AgendaItemActionId.ToString();
//        }

//        protected virtual AgendaItemActionAttachment PopulateDbFromDomainModel(AgendaItemActionAttachment entityModel, DocumentActionAttachment data)
//        {
//            //entityModel.Id = data.Id;
//            entityModel.AgendaItemActionId = data.AgendaItemActionId;
//            entityModel.DocumentId = data.AttachmentId;
//            //entityModel.MimeType = data.MimeType;
//            //entityModel.Size = data.Size;
//            //entityModel.StorageType = data.StorageType;
//            //entityModel.StoragePath = data.StoragePath;

//            return entityModel;
//        }

//        public IQueryable<DocumentActionAttachment> GetActionAttachments()
//        {
//            var context = ContextFactory();

//            return from p in context.AgendaItemActionAttachment
//                   select new DocumentActionAttachment
//                   {
//                       AgendaItemActionId = p.AgendaItemActionId,
//                       AttachmentId = p.DocumentId,
//                       //Content = p.Content,
//                       //MimeType = p.MimeType,
//                       //Size = p.Size,
//                       //StorageType = p.StorageType,
//                       //UploadedOn = p.UpdatedOn,
//                       //StoragePath = p.StoragePath,
//                   };
//        }
//        #endregion


//        public async Task<IEnumerable<DocumentAttachment>> GetAgendaItemAttachments(int meetingId, string query)
//        {
//            var context = ContextFactory();
//            var agendaItemAttachments =
//               (from m in context.Meeting
//                join ma in context.MeetingAgenda on m.Id equals ma.MeetingId
//                join ai in context.AgendaItem on ma.AgendaItemId equals ai.Id
//                join aia in context.AgendaItemAttachment on ai.Id equals aia.AgendaItemId
//                where m.Id == meetingId && aia.AttachmentType == Shared.Enums.Declares.AttachmentType.File
//                orderby ai.ParentId.HasValue ? ai.Parent.OrderNo.ToString() + "." + ai.OrderNo : ai.OrderNo.ToString() + '.'
//                select new DocumentAttachment
//                {
//                    //AgendaItemParentId = ai.ParentId.HasValue ? ai.ParentId : null,
//                    AgendaItemOrderNo = aia.AgendaItem.ParentId.HasValue ? aia.AgendaItem.Parent.OrderNo.ToString() + "." + aia.AgendaItem.OrderNo : aia.AgendaItem.OrderNo.ToString() + '.',
//                    AgendaItemName = aia.AgendaItem.Title,
//                    AgendaItemId = aia.AgendaItemId,
//                    AttachmentId = aia.DocumentId,
//                    DocumentName = aia.Document.Name,
//                    DocumentType = Path.GetExtension(aia.Document.Name),
//                    AgendaItemDocumentId = aia.AgendaItemDocumentId,
//                    UserName = aia.UpdatedBy
//                }).AsEnumerable();
//            if (!string.IsNullOrEmpty(query))
//            {
//                string queryFormatted = query.ToLower();
//                agendaItemAttachments = agendaItemAttachments.Where(x => x.DocumentName.ToLower().Contains(queryFormatted) || x.AgendaItemOrderNo.ToLower().Contains(queryFormatted)
//                || x.AgendaItemName.ToLower().Contains(queryFormatted)).ToList();
//            }

//            return await Task.FromResult(agendaItemAttachments);
//        }

//        public async Task<IEnumerable<DocumentAttachment>> GetDocumentAttachments(int agendaItemId, string query)
//        {
//            var context = ContextFactory();
//            var attachments =
//               (from ai in context.AgendaItem
//                join aid in context.AgendaItemDocument on ai.Id equals aid.AgendaItemId
//                join aia in context.AgendaItemAttachment on aid.Id equals aia.AgendaItemDocumentId
//                where ai.Id == agendaItemId && aia.AttachmentType == Shared.Enums.Declares.AttachmentType.File

//                select new DocumentAttachment
//                {
//                    //AgendaItemParentId = ai.ParentId.HasValue ? ai.ParentId : null,
//                    AgendaItemOrderNo = aia.AgendaItem.ParentId.HasValue ? aia.AgendaItem.Parent.OrderNo.ToString() + "." + aia.AgendaItem.OrderNo : aia.AgendaItem.OrderNo.ToString() + '.',
//                    AgendaItemName = aia.AgendaItem.Title,
//                    AgendaItemDocumentName = aid.Name,
//                    AgendaItemId = aia.AgendaItemId,
//                    AttachmentId = aia.DocumentId,
//                    DocumentName = aia.Document.Name,
//                    DocumentType = Path.GetExtension(aia.Document.Name),
//                    AgendaItemDocumentId = aia.AgendaItemDocumentId,
//                    UserName = aia.UpdatedBy
//                }).AsEnumerable();

//            if (!string.IsNullOrEmpty(query))
//            {
//                string queryFormatted = query.ToLower();
//                attachments = attachments.Where(x => x.DocumentName.ToLower().Contains(queryFormatted) || x.AgendaItemOrderNo.ToLower().Contains(queryFormatted)
//                || x.AgendaItemName.ToLower().Contains(queryFormatted)).ToList();
//            }
//            return await Task.FromResult(attachments);
//        }

//        public async Task<IEnumerable<DocumentAttachment>> GetSectionAttachments(int agendaItemId)
//        {
//            var context = ContextFactory();
//            var attachments =
//               (from ai in context.AgendaItem
//                join ais in context.AgendaItemSection on ai.Id equals ais.AgendaItemId
//                join aia in context.AgendaItemAttachment on ais.Id equals aia.AgendaItemSectionId
//                where ai.Id == agendaItemId && aia.AttachmentType == Shared.Enums.Declares.AttachmentType.File

//                select new DocumentAttachment
//                {
//                    //AgendaItemParentId = ai.ParentId.HasValue ? ai.ParentId : null,
//                    AgendaItemOrderNo = aia.AgendaItem.ParentId.HasValue ? aia.AgendaItem.Parent.OrderNo.ToString() + "." + aia.AgendaItem.OrderNo : aia.AgendaItem.OrderNo.ToString() + '.',
//                    AgendaItemName = aia.AgendaItem.Title,
//                    AgendaItemId = aia.AgendaItemId,
//                    AttachmentId = aia.DocumentId,
//                    DocumentName = aia.Document.Name,
//                    DocumentType = Path.GetExtension(aia.Document.Name),
//                    AgendaItemSectionId = aia.AgendaItemSectionId,
//                    UserName = aia.UpdatedBy,
//                    SectionName = ais.Name
//                }).AsEnumerable();

//            return await Task.FromResult(attachments);
//        }


//    }
//}
