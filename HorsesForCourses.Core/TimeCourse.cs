namespace HorsesForCourses.Core;


public class TimeSlot
{
    public WeekDay Day { get; }
    public int Start { get; }
    public int End { get; }

    public TimeSlot(WeekDay day, int start, int end)
    {
        Day = day;
        Start = start;
        End = end;

        if (End <= Start) throw new ArgumentException("You can't finish a course that hasn't started yet.");
    }
}

public class Period
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public Period(DateTime start, DateTime end)
    {
        if (start.Date > end.Date) throw new ArgumentException("Start date must be before or equal to end date.");

        StartDate = start.Date;
        EndDate = end.Date;
    }

    // public bool OverlapsWith(Period other)
    // {
    //     return StartDate <= other.EndDate && EndDate >= other.StartDate;
    // }
}
