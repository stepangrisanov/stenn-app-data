using Microsoft.EntityFrameworkCore;
using Stenn.AppData.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stenn.AppData
{
    public class ExpressionTreeValidator<T> : ExpressionVisitor
    {
        private readonly List<MethodInfo> _allowedMethods = new();
        private readonly Func<MethodInfo, bool>? _additionalValidationFunc;

        public ExpressionTreeValidator(Func<MethodInfo, bool>? additionalValidationFunc = null)
        {
            _additionalValidationFunc = additionalValidationFunc;
            _allowedMethods.AddRange(typeof(IAppDataService<>).GetMethods().Where(i => i.Name == "Query"));
            _allowedMethods.AddRange(typeof(Queryable).GetMethods());
            _allowedMethods.AddRange(typeof(EntityFrameworkQueryableExtensions).GetMethods());
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            if (!CheckAllowed(method) && !CheckAllowedExternally(method))
            {
                throw new InvalidOperationException($"Expression contains not allowed method {method.Name} from module {method.Module}. Use DI method to add custom validation function if needed.");
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

        private bool CheckAllowed(MethodInfo method)
        {
            return _allowedMethods.Where(i => i.MetadataToken == method.MetadataToken && i.Module == method.Module).Count() != 0;
        }

        private bool CheckAllowedExternally(MethodInfo method)
        {
            return _additionalValidationFunc?.Invoke(method) ?? false;
        }
    }
}
