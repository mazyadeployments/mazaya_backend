using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MMA.WebApi.DataAccess.Extensions;
using MMA.WebApi.Shared.Helpers;
using MMA.WebApi.Shared.Interfaces.Tag;
using MMA.WebApi.Shared.Models.GenericData;
using MMA.WebApi.Shared.Models.Tag;
using MMA.WebApi.Shared.Monads;
using MMA.WebApi.Shared.Visitor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMA.WebApi.Core.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private HttpContext httpcontext;
        public TagService(ITagRepository tagRepository, IHttpContextAccessor httpaccess)
        {
            _tagRepository = tagRepository;
            httpcontext = httpaccess.HttpContext;
        }
        public async Task<PaginationListModel<TagModel>> GetTagsPage(QueryModel queryModel)
        {
            var tags = _tagRepository.GetTagsPage(queryModel);
            return await tags.ToPagedListAsync(queryModel.PaginationParameters.PageNumber, queryModel.PaginationParameters.PageSize);
        }

        public async Task<IEnumerable<TagModel>> GetTags()
        {
            var tags = await _tagRepository.Get().ToListAsync();

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
        public async Task<Maybe<TagModel>> GetTag(int id)
        {
            return await _tagRepository.Get(id);
        }

        public async Task<IEnumerable<TagModel>> GetTagsMobile()
        {
            return await _tagRepository.Get().ToListAsync();
        }

        public async Task<Maybe<TagModel>> CreateTag(TagModel model, string userId)
        {
            var auditVisitor = new CreateAuditVisitor(userId, DateTime.UtcNow);
            return await _tagRepository.CreateAsync(model, auditVisitor);
        }

        public async Task DeleteTag(int id)
        {
            await _tagRepository.DeleteAsync(id);
        }
    }
}
