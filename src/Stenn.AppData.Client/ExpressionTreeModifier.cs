using System.Linq.Expressions;

namespace Stenn.AppData.Client
{
    internal class ExpressionTreeModifier : ExpressionVisitor
    {
        private readonly Expression _serviceParam;

        public ExpressionTreeModifier(Expression serviceParam)
        {
            _serviceParam = serviceParam;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (!IsSubclassOfRawGeneric(typeof(Query<>), c.Type))
            {
                return c;
            }

            var serviceQueryType = c.Type.GetGenericArguments()[0];
            var serviceCall = Expression.Call(_serviceParam, "Query", new[] { serviceQueryType });
            return serviceCall;
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type? toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType!;
            }
            return false;
        }
    }
}
