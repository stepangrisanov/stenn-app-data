using System.Linq;

namespace Stenn.AppData.Contracts.RequestOptions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Filter filter)
        {
            if (filter is null) return source;

            var filterExpression = new FilterExpressionBuilder<TSource>().GetFilterExpression(filter);

            return source.Where(filterExpression);
        }
    }
}
