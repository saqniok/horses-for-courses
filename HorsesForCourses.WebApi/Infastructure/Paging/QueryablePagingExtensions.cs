public static class QueryablePagingExtensions
{
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, PageRequest request)
    {
        //Defensive: If there is no orderby, fall back in a stable order (eg Primary Key)
        //This requires that T has an ID; Otherwise you inject an orderby elsewhere.
        if (!query.Expression.ToString().Contains("OrderBy"))
            throw new InvalidOperationException("Apply an OrderBy before paging to ensure stable results.");

        int skip = (request.Page - 1) * request.Size;
        return query.Skip(skip).Take(request.Size);
    }
}