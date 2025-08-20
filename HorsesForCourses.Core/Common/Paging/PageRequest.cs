// DTO
public sealed record PageRequest(int PageNumber = 1, int PageSize = 5)
{
    public int Page => PageNumber < 1 ? 1 : PageNumber;
    public int Size => PageSize is < 1 ? 1 : (PageSize > 5 ? 5 : PageSize); // guardrails
}

// Domain
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,     // List of objects (Coaches, Courses, etc.)
    int TotalCount,             // Total number of objects
    int PageNumber,             // Current page
    int PageSize)               // Number of objects per page
{
    // Math.Ceiling return the smallest integer >= to the given value
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize); // Total number of pages: Ex: 11/5 = Math.Ceiling(2.2) => 3
    public bool HasPrevious => PageNumber > 1; // If Current page > 1 return true, it's means you can get previous page objects
    public bool HasNext => PageNumber < TotalPages; // If Current page < Total pages return true, it's means you can get next page objects
}