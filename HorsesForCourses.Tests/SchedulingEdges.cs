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

    [Fact]
    public void No_overlap_if_same_weekday_but_different_dates()
    {
        var coach = new Coach("Test Coach", "test@example.com");

        // Course One: Tuesday, August 20, 2025, 9-10 AM
        var courseOnePeriod = new TimeDay(new DateOnly(2025, 8, 18), new DateOnly(2025, 8, 24)); // Week of Aug 18-24
        var courseOne = new Course("Course Alpha", courseOnePeriod);
        courseOne.AddTimeSlot(new TimeSlot(WeekDay.Tuesday, 9, 10));
        courseOne.Confirm();
        coach.AssignCourse(courseOne);

        // Course Two: Tuesday, August 27, 2025, 9-10 AM (different week)
        var courseTwoPeriod = new TimeDay(new DateOnly(2025, 8, 25), new DateOnly(2025, 8, 31)); // Week of Aug 25-31
        var courseTwo = new Course("Course Beta", courseTwoPeriod);
        courseTwo.AddTimeSlot(new TimeSlot(WeekDay.Tuesday, 9, 10));
        courseTwo.Confirm();

        // This assignment should NOT throw an exception
        var exception = Record.Exception(() => coach.AssignCourse(courseTwo));
        Assert.Null(exception);
        Assert.Contains(courseTwo, coach.AssignedCourses);
    }
}
