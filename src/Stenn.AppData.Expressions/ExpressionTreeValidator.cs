using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.Expressions
{
    public class ExpressionTreeValidator<TBaseEntity> : ExpressionVisitor
        where TBaseEntity : IAppDataEntity
    {
        private readonly List<MethodInfo> _allowedMethods = new();

        public ExpressionTreeValidator()
        {
            _allowedMethods.AddRange(typeof(IAppDataService<>).GetMethods().Where(i => i.Name == "Query"));
            _allowedMethods.AddRange(typeof(Queryable).GetMethods());
            _allowedMethods.AddRange(typeof(EntityFrameworkQueryableExtensions).GetMethods());
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!CheckAllowed(node.Method))
            {
                throw new InvalidOperationException(
                    $"Expression contains not allowed method {node.Method.Name} from module {node.Method.Module}. Use DI method to add custom validation function if needed.");
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            return Expression.Empty();
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            return Expression.Empty();
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return Expression.Empty();
        }

        protected virtual bool CheckAllowed(MethodInfo method)
        {
            return _allowedMethods.Any(i => i.MetadataToken == method.MetadataToken && i.Module == method.Module);
        }
    }
}