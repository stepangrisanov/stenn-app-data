using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stenn.AppData.Client
{
    public static class QueryableExtensions
    {
        internal static readonly MethodInfo IncludeMethodInfo
            = typeof(EntityFrameworkQueryableExtensions)
                .GetTypeInfo().GetDeclaredMethods("Include")
                .Single(
                    mi =>
                        mi.GetGenericArguments().Count() == 2
                        && mi.GetParameters().Any(
                            pi => pi.Name == "navigationPropertyPath" && pi.ParameterType != typeof(string)));

        public static IIncludableQueryable<TEntity, TProperty> IncludeSerializable<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> navigationPropertyPath)
            where TEntity : class
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(navigationPropertyPath, nameof(navigationPropertyPath));

            return new IncludableQuery<TEntity, TProperty>(
                    source.Provider.CreateQuery<TEntity>(
                        Expression.Call(
                            instance: null,
                            method: IncludeMethodInfo.MakeGenericMethod(typeof(TEntity), typeof(TProperty)),
                            arguments: new[] { source.Expression, Expression.Quote(navigationPropertyPath) }))
                    );
        }
    }
}
