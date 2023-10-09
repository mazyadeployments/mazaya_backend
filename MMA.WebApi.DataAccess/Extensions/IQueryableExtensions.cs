using Microsoft.EntityFrameworkCore;
using MMA.WebApi.Shared.Models.GenericData;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MMA.WebApi.DataAccess.Extensions
{
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "OrderBy", propertyName, comparer);
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "OrderByDescending", propertyName, comparer);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "ThenBy", propertyName, comparer);
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "ThenByDescending", propertyName, comparer);
        }

        /// <summary>
        /// Builds the Queryable functions using a TSource property name.
        /// Property name can be simple as follows: query = query.OrderBy("ProductId"); or a nested one as follows: query = query.OrderBy("ProductCategory.CategoryId").
        /// If we are not using Entity Framework or Linq to Sql, this works: query = query.OrderBy("ProductCategory", comparer). 
        /// Caution: The IComparer parameter does not work with Entity Framework and should be left out if using Linq to Sql.
        /// </summary>
        public static IOrderedQueryable<T> CallOrderedQueryable<T>(this IQueryable<T> query, string methodName, string propertyName, IComparer<object> comparer = null)
        {
            var param = Expression.Parameter(typeof(T), "x");

            var body = propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

            return comparer != null
                ? (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param),
                        Expression.Constant(comparer)
                    )
                )
                : (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param)
                    )
                );
        }

        /// <summary>
        /// Similar to built in ToList() function, except does paging and adds paging informations. Executes query.
        /// </summary>
        /// <typeparam name="T">Type of list</typeparam>
        /// <param name="query">IQueryable of T</param>
        /// <param name="pageIndex">Requested zero based paged index</param>
        /// <param name="pageSize">Requested page size</param>
        /// <returns>Paginated List with page info</returns>
        /// <remarks>Query is executed against database </remarks>
        public static PaginationListModel<T> ToPaginatedList<T>(this IQueryable<T> query, int pageIndex, int pageSize) where T : class
        {
            var totalCount = query.Count();

            var collection = query.AsNoTracking()
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginationListModel<T>();
        }
    }
}
