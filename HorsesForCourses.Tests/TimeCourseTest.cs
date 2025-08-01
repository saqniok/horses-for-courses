using HorsesForCourses.Core;
namespace HorsesForCourses.Test;

public class TimeCourseTest
{

    [Fact]
    public void TimeSlot_Constructor()
    {
        var timeslot = new TimeSlot(WeekDay.Monday, 10, 12);

        Assert.Equal(10, timeslot.Start);
        Assert.Equal(12, timeslot.End);
        Assert.Equal(2, timeslot.Duration);
        Assert.Equal(WeekDay.Monday, timeslot.Day);
    }

    [Fact]
    public void TimeSlot_Exceptions()
    {
        var afterWorkingHours = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 9, 20));
        Assert.Equal("Lesson time must be within working hours 9:00 - 17:00", afterWorkingHours.Message);

        var beforeWorkingHours = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 8, 17));
        Assert.Equal("Lesson time must be within working hours 9:00 - 17:00", beforeWorkingHours.Message);

        var EndHourBiggerThenStart = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 20, 8));
        Assert.Equal("Lesson end time must be after start time", EndHourBiggerThenStart.Message);

        var LessThenHour = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Friday, 10, 10));
        Assert.Equal("Lesson duration must be at least 1 hour.", LessThenHour.Message);
    }

    [Fact]
    public void OverlapWithOtherTime()
    {
        var Course1 = new TimeSlot(WeekDay.Monday, 9, 12);
        var Course2 = new TimeSlot(WeekDay.Monday, 9, 10);
        var Course3 = new TimeSlot(WeekDay.Monday, 11, 15);
        var Course4 = new TimeSlot(WeekDay.Friday, 9, 17);

        Assert.False(Course2.OverlapsWith(Course3));
        Assert.True(Course1.OverlapsWith(Course2));
        Assert.True(Course1.OverlapsWith(Course3));
        Assert.False(Course1.OverlapsWith(Course4));
    }
}

public class PeriodTest
{
    [Fact]
    public void PeriodTest_Constructor()
    {
        var start = new DateOnly(2025, 6, 1);
        var end = new DateOnly(2025, 6, 7);
        var date = new Period(start, end);

        Assert.Equal(start, date.StartDate);
        Assert.Equal(end, date.EndDate);

        var test = new DateOnly(2024, 6, 7);
        var exception = Assert.Throws<ArgumentException>(() => new Period(start, test));

        Assert.Equal("Start date must be before or equal to end date.", exception.Message);
    }
}
