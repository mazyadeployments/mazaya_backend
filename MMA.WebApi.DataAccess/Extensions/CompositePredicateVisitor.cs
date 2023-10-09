using System.Collections.Generic;
using System.Linq.Expressions;

namespace MMA.WebApi.DataAccess.Extensions
{
    class CompositePredicateVisitor : ExpressionVisitor
    {
        readonly Dictionary<ParameterExpression, ParameterExpression> map;

        CompositePredicateVisitor(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new CompositePredicateVisitor(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;

            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }
}
