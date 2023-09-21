using Microsoft.EntityFrameworkCore;
using MMA.WebApi.Shared.Models.GenericData;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MMA.WebApi.DataAccess.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Execute query and transform result to paged list
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="query">query</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <returns>paged data from query</returns>
        public static PaginationListModel<T> ToPagedList<T>(
            this IQueryable<T> query,
            int pageIndex,
            int pageSize
        )
            where T : class
        {
            var totalCount = query.Count();

            var collection = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginationListModel<T>()
            {
                List = collection,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPageCount = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0
            };
        }

        /// <summary>
        /// Execute query and transform result to paged list
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="query">query</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <returns>paged data from query</returns>
        public static async Task<PaginationListModel<T>> ToPagedListAsync<T>(
            this IQueryable<T> query,
            int pageIndex,
            int pageSize
        )
            where T : class
        {
            var totalCount = await query.CountAsync();

            var collection = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationListModel<T>()
            {
                List = collection,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPageCount = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0
            };
        }
    }
}
