namespace HorsesForCourses.Core;

// Lesson Duration in hours
public class TimeSlot
{
    public WeekDay Day { get; }
    public int Start { get; }
    public int End { get; }
    
    private TimeSlot() { }

    public TimeSlot(WeekDay day, int start, int end)
    {
        if (start < 9 || end > 17)
            throw new ArgumentException("Lesson time must be within working hours 9:00 - 17:00");

        if (end < start)
            throw new ArgumentException("Lesson end time must be after start time");

        if (end - start < 1)
            throw new ArgumentException("Lesson duration must be at least 1 hour.");

        Day = day;
        Start = start;
        End = end;
    }

    public int Duration => End - Start;

    public bool OverlapsWith(TimeSlot other)
    {
        if (Day != other.Day) return false;

        return Start <= other.End && End >= other.Start;
    }

}
