using System.Linq;

namespace Stenn.AppData.Contracts.RequestOptions
{
    public static class PagingExpressionBuilder
    {
        public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> source, PagingOptions pagingOptions)
        {
            if (pagingOptions is null) return source;

            source = source.Take(pagingOptions.Take);

            return source;
        }
    }
}
