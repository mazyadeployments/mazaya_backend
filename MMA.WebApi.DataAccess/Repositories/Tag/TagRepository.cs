using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Models;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.GenericData;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Models.Tag;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Tags
{
    public class TagRepository : BaseRepository<TagModel>, ITagRepository
    {
        public TagRepository(Func<MMADbContext> contexFactory) : base(contexFactory) { }
        public IQueryable<TagModel> Get()
        {
            var context = ContextFactory();

            return context.Tag
                .Select(projectToTagModel);
        }
        public IQueryable<TagModel> GetTagsPage(QueryModel queryModel)
        {
            var context = ContextFactory();

            IQueryable<Tag> filteredTag = null;
            IQueryable<TagModel> tagModels = null;

            var tags = context.Tag.AsNoTracking();

            filteredTag = Filter(tags, queryModel);
            tagModels = filteredTag.Select(projectToTagModel);

            return Sort(queryModel.Sort, tagModels);
        }

        private static IQueryable<Tag> Filter(IQueryable<Tag> tags, QueryModel queryModel)
        {
            var filteredTag = tags.Where(tag => tag.Title.ToLower().Contains(queryModel.Filter.Keyword.Trim().ToLower()));

            return filteredTag;
        }
        private static IQueryable<TagModel> Sort(SortModel sortModel, IQueryable<TagModel> collection)
        {
            if (sortModel.Type == "date")
            {
                if (sortModel.Order == Order.DESC)
                {
                    return collection.OrderByDescending(x => x.UpdatedOn);
                }
                else
                {
                    return collection.OrderBy(x => x.UpdatedOn);
                }
            }
            else
            {
                return collection.OrderByDescending(x => x.UpdatedOn);
            }
        }

        protected override IQueryable<TagModel> GetEntities()
        {
            throw new NotImplementedException();
        }
        public async Task<TagModel> CreateAsync(TagModel model, IVisitor<IChangeable> auditVisitor)
        {
            var context = ContextFactory();

            var tag = context.Tag.Include(o => o.OfferTags).FirstOrDefault(x => x.Id == model.Id);

            if (tag == null)
                tag = new Tag();

            PopulateEntityModel(tag, model);

            if (model.Id == 0)
            {
                context.Add(tag);
            }
            else
            {
                tag.UpdatedOn = DateTime.UtcNow;
                context.Update(tag);
            }

            await context.SaveChangesAsync();

            return projectToTagModel.Compile().Invoke(tag);
        }
        public async Task<TagModel> Get(int id)
        {
            var context = ContextFactory();

            return await context.Tag
                    .Select(projectToTagModel)
                    .FirstOrDefaultAsync(x => x.Id == id);
        }

        private Expression<Func<Tag, TagModel>> projectToTagModel = data =>
           new TagModel()
           {
               Id = data.Id,
               Title = data.Title,
               IsEditable = data.IsEditable,
               //OffersIds = data.OfferTags.Select(x => x.OfferId).ToList(),
               CreatedBy = data.CreatedBy,
               CreatedOn = data.CreatedOn,
               UpdatedBy = data.UpdatedBy,
               UpdatedOn = data.UpdatedOn,
               Description = data.Description
           };

        private void PopulateEntityModel(Tag data, TagModel model)
        {
            data.Id = model.Id;
            data.Title = model.Title;
            data.IsEditable = model.IsEditable;
            if (model.OffersIds != null)
            {
                data.OfferTags = model.OffersIds.Select(offerId => new OfferTag { OfferId = offerId, TagId = model.Id }).ToList();
            }
            data.Description = model.Description;
        }

        public async Task<TagModel> DeleteAsync(int id)
        {
            var context = ContextFactory();
            var tag = await context.Tag
                        .AsNoTracking()
                        .Select(projectToTagModel)
                        .FirstOrDefaultAsync(x => x.Id == id);

            if (tag != null)
            {
                var data = new Tag();
                data.Id = tag.Id;

                context.Remove(data);
                context.SaveChanges();
            }
            return tag;
        }

        public async Task<int> GetTagsCount()
        {
            var context = ContextFactory();

            return await context.Tag.CountAsync();
        }

        public async Task<IEnumerable<TagModel>> GetTags()
        {
            var tags = await Get().ToListAsync();

            //Remove tags that user cannot select like 'Latest', 'Ending Soon', 'Upcoming'
            var tagsWithoutTimeLimit = new List<TagModel>();

            foreach (var tag in tags)
            {
                if (!string.Equals(tag.Title, "Latest") && (!string.Equals(tag.Title, "Ending Soon") && (!string.Equals(tag.Title, "Upcoming"))))
                {
                    tagsWithoutTimeLimit.Add(tag);
                }
            }

            return tagsWithoutTimeLimit;
        }
    }
}
