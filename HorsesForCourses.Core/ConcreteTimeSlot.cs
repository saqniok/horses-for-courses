namespace HorsesForCourses.Core;

public class ConcreteTimeSlot
{
    public DateOnly Date { get; }
    public int Start { get; }
    public int End { get; }

    public ConcreteTimeSlot(DateOnly date, int start, int end)
    {
        Date = date;
        Start = start;
        End = end;
    }
}
