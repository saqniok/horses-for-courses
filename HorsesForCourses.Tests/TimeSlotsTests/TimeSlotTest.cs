using System;
using Xunit;
using HorsesForCourses.Core;

public class TimeSlotTests
{
    [Fact]
    public void Constructor_StartBefore9_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 8, 10));
        Assert.Equal("Lesson time must be within working hours 9:00 - 17:00", ex.Message);
    }

    [Fact]
    public void Constructor_EndAfter17_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 10, 18));
        Assert.Equal("Lesson time must be within working hours 9:00 - 17:00", ex.Message);
    }

    [Fact]
    public void Constructor_EndBeforeStart_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 12, 10));
        Assert.Equal("Lesson end time must be after start time", ex.Message);
    }

    [Fact]
    public void Constructor_DurationLessThan1_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 10, 10));
        Assert.Equal("Lesson duration must be at least 1 hour.", ex.Message);
    }

    [Fact]
    public void Constructor_ValidTime_CreatesTimeSlot()
    {
        var ts = new TimeSlot(WeekDay.Monday, 10, 12);
        Assert.Equal(WeekDay.Monday, ts.Day);
        Assert.Equal(10, ts.Start);
        Assert.Equal(12, ts.End);
    }

    [Fact]
    public void OverlapsWith_DifferentDay_ReturnsFalse()
    {
        var ts1 = new TimeSlot(WeekDay.Monday, 10, 12);
        var ts2 = new TimeSlot(WeekDay.Tuesday, 10, 12);
        Assert.False(ts1.OverlapsWith(ts2));
    }

    [Fact]
    public void OverlapsWith_OverlappingTimes_ReturnsTrue()
    {
        var ts1 = new TimeSlot(WeekDay.Monday, 10, 12);
        var ts2 = new TimeSlot(WeekDay.Monday, 11, 13);
        Assert.True(ts1.OverlapsWith(ts2));
    }

    [Fact]
    public void OverlapsWith_NonOverlappingTimes_ReturnsFalse()
    {
        var ts1 = new TimeSlot(WeekDay.Monday, 10, 12);
        var ts2 = new TimeSlot(WeekDay.Monday, 13, 14);
        Assert.False(ts1.OverlapsWith(ts2));
    }
}
