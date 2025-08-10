using System;
using Xunit;
using HorsesForCourses.Core;

public class TimeSlotTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenStartBefore9()
    {
        var ex = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 8, 10));
        Assert.Equal("Lesson time must be within working hours 9:00 - 17:00", ex.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenEndAfter17()
    {
        var ex = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 9, 18));
        Assert.Equal("Lesson time must be within working hours 9:00 - 17:00", ex.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenEndBeforeStart()
    {
        var ex = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 11, 10));
        Assert.Equal("Lesson end time must be after start time", ex.Message);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenDurationLessThan1()
    {
        var ex = Assert.Throws<ArgumentException>(() => new TimeSlot(WeekDay.Monday, 10, 10));
        Assert.Equal("Lesson duration must be at least 1 hour.", ex.Message);
    }

    [Fact]
    public void Duration_ShouldReturnCorrectValue()
    {
        var slot = new TimeSlot(WeekDay.Tuesday, 9, 12);
        Assert.Equal(3, slot.Duration);
    }

    [Fact]
    public void OverlapsWith_ShouldReturnTrue_WhenSlotsOverlapOnSameDay()
    {
        var slot1 = new TimeSlot(WeekDay.Wednesday, 10, 12);
        var slot2 = new TimeSlot(WeekDay.Wednesday, 11, 13);

        Assert.True(slot1.OverlapsWith(slot2));
        Assert.True(slot2.OverlapsWith(slot1));
    }

    [Fact]
    public void OverlapsWith_ShouldReturnFalse_WhenSlotsDoNotOverlapOnSameDay()
    {
        var slot1 = new TimeSlot(WeekDay.Thursday, 9, 10);
        var slot2 = new TimeSlot(WeekDay.Friday, 10, 11);

        Assert.False(slot1.OverlapsWith(slot2));
        Assert.False(slot2.OverlapsWith(slot1));
    }

    [Fact]
    public void OverlapsWith_ShouldReturnFalse_WhenSlotsOnDifferentDays()
    {
        var slot1 = new TimeSlot(WeekDay.Friday, 9, 12);
        var slot2 = new TimeSlot(WeekDay.Monday, 9, 12);

        Assert.False(slot1.OverlapsWith(slot2));
    }
}
