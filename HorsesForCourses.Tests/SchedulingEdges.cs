using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class SchedulingEdges
{
    [Fact(Skip = "Unskip for failure")]
    public void Overlaps_on_friday_but_we_only_teach_on_thursday()
    {
        var coach = new Coach("a", "a@a");

        var courseOne = new Course("One", new TimeDay(new DateOnly(2025, 8, 21), new DateOnly(2025, 8, 21)));
        courseOne.AddTimeSlot(new TimeSlot(WeekDay.Friday, 9, 10));
        courseOne.Confirm();
        courseOne.AssignCoach(coach);

        var courseTwo = new Course("One", new TimeDay(new DateOnly(2025, 8, 21), new DateOnly(2025, 8, 21)));
        courseTwo.AddTimeSlot(new TimeSlot(WeekDay.Friday, 9, 10));
        courseTwo.Confirm();

        courseTwo.AssignCoach(coach);


        Assert.Equal(DayOfWeek.Thursday, new DateOnly(2025, 8, 21).DayOfWeek);
        Assert.Equal(coach, courseTwo.AssignedCoach);
    }
}
