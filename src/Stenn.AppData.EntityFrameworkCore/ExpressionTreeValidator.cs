using Microsoft.EntityFrameworkCore;
using Stenn.AppData.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stenn.AppData
{
    public class ExpressionTreeValidator : ExpressionVisitor
    {
        private readonly List<MethodInfo> _allowedMethods = new List<MethodInfo>();

        public ExpressionTreeValidator()
        {
            _allowedMethods.AddRange(typeof(IAppDataService<>).GetMethods().Where(i => i.Name == "Query"));
            _allowedMethods.AddRange(typeof(Queryable).GetMethods());
            _allowedMethods.AddRange(typeof(EntityFrameworkQueryableExtensions).GetMethods());
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var method = node.Method;
            if (_allowedMethods.Where(i => i.MetadataToken == method.MetadataToken && i.Module == method.Module).Count() == 0)
            {
                return Expression.Empty();
            }

            return base.VisitMethodCall(node);
        }
    }
}
