using System;
using System.Linq;
using System.Linq.Expressions;

namespace MMA.WebApi.DataAccess.Extensions
{
    public static partial class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() { return param => true; }

        public static Expression<Func<T, bool>> False<T>() { return param => false; }

        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) { return predicate; }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second, Func<bool> cond)
        {
            return cond.Invoke() ? first.Compose(second, Expression.AndAlso) : first;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second, Func<bool> cond)
        {
            return cond.Invoke() ? first.Compose(second, Expression.OrElse) : first;
        }

        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.Not(expression.Body),
                expression.Parameters);
        }

        static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var secondBody = CompositePredicateVisitor.ReplaceParameters(
                first.Parameters
                    .Select((f, i) => new { f, s = second.Parameters[i] })
                    .ToDictionary(p => p.s, p => p.f),
                second.Body);

            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
    }
}
