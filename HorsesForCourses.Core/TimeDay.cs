namespace HorsesForCourses.Core;


// Course Durection class
public class TimeDay
{
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    protected TimeDay(DateTime today, DateTime dateTime) { }

    public TimeDay(DateOnly start, DateOnly end)
    {
        if (start > end) throw new ArgumentException("Start date must be before or equal to end date.");

        StartDate = start;
        EndDate = end;
    }

    public bool OverlapsWith(TimeDay other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }
}
