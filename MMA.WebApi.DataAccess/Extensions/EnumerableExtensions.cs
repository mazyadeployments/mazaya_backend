using MMA.WebApi.Shared.Models.GenericData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMA.WebApi.DataAccess.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Execute query and transform result to paged list
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="query">query</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <returns>paged data from query</returns>
        public static PaginationListModel<T> ToPagedList<T>(this IEnumerable<T> query, int pageIndex, int pageSize) where T : class
        {
            var totalCount = query.Count();

            var collection = query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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
