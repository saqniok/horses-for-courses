namespace HorsesForCourses.Service.DTOs;

public sealed record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    bool HasPrevious,
    bool HasNext
);
