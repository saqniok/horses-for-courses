using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HorsesForCourses.Service.Data;

// Helper: converts HashSet<string> <-> ";" separated string
public static class Converters
{
    public static ValueConverter<HashSet<string>, string> HashSetToString()
        => new ValueConverter<HashSet<string>, string>(
            v => string.Join(';', v),
            v => string.IsNullOrWhiteSpace(v)
                    ? new HashSet<string>()
                    : v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToHashSet()
        );

    public static ValueComparer<HashSet<string>> HashSetComparer()
        => new ValueComparer<HashSet<string>>(
        (c1, c2) => (c1 ?? new HashSet<string>()).SetEquals(c2 ?? new HashSet<string>()),
            c => (c ?? new HashSet<string>()).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c == null ? new HashSet<string>() : new HashSet<string>(c)
        );

    // public static ValueConverter<List<string>, string> ListToString()
    //     => new ValueConverter<List<string>, string>(
    //         v => string.Join(';', v),
    //         v => string.IsNullOrWhiteSpace(v)
    //                 ? new List<string>()
    //                 : v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()
    //     );

    // public static ValueComparer<List<string>> ListComparer()
    //     => new ValueComparer<List<string>>(
    //         (c1, c2) => (c1 ?? new List<string>()).SequenceEqual(c2 ?? new List<string>()),
    //         c => (c ?? new List<string>()).Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
    //         c => c == null ? new List<string>() : c.ToList()
    //     );

}