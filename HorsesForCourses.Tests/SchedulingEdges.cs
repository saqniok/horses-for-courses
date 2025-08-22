using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class SchedulingEdges
{
    [Fact]
    public void Overlaps_on_friday_but_we_only_teach_on_thursday()
    {
        var coach = new Coach("a", "a@a");

        var courseOne = new Course("One", new TimeDay(new DateOnly(2025, 8, 21), new DateOnly(2025, 8, 21)));
        courseOne.AddTimeSlot(new TimeSlot(WeekDay.Thursday, 9, 10));
        courseOne.Confirm();
        courseOne.AssignCoach(coach);

        var courseTwo = new Course("One", new TimeDay(new DateOnly(2025, 8, 21), new DateOnly(2025, 8, 21)));
        courseTwo.AddTimeSlot(new TimeSlot(WeekDay.Thursday, 10, 12));
        courseTwo.Confirm();

        courseTwo.AssignCoach(coach);


        Assert.Equal(DayOfWeek.Thursday, new DateOnly(2025, 8, 21).DayOfWeek);
        Assert.Equal(coach, courseTwo.AssignedCoach);
    }

    [Fact]
    public void Throw_exeption_if_overlaps_on_day_that_period_does_not_include()
    {
        var coach = new Coach("a", "a@a");

        var courseOne = new Course("One", new TimeDay(new DateOnly(2025, 8, 21), new DateOnly(2025, 8, 21)));
        courseOne.AddTimeSlot(new TimeSlot(WeekDay.Thursday, 9, 10));

        var ex = Assert.Throws<InvalidOperationException>(() => courseOne.AddTimeSlot(new TimeSlot(WeekDay.Friday, 9, 10)));

        Assert.Equal("Cannot add a time slot for a day that is not included in the course duration.", ex.Message);
    }
}
