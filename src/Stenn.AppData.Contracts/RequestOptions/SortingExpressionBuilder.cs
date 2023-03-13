using System;
using System.Linq;
using System.Linq.Expressions;

namespace Stenn.AppData.Contracts.RequestOptions
{
    public static class SortingExpressionBuilder
    {
        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, SortOptions sortOptions)
        {
            if (sortOptions is null || sortOptions.Sorting is null) return source;

            bool initialSort = true;
            foreach (var item in sortOptions.Sorting)
            {
                source = source.OrderBy(item.FieldName, item.SortDirection == SortDirection.Ascending, initialSort);
                initialSort = false;
            }

            return source;
        }

        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc, bool initialSort)
        {
            string command;

            if (initialSort)
            {
                command = desc ? "OrderByDescending" : "OrderBy";
            }
            else
            {
                command = desc ? "ThenByDescending" : "ThenBy";
            }
            
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty)!;
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}
