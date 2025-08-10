using HorsesForCourses.Core;

namespace HorsesForCourses.Test;

public class CoachTests
{
    [Fact]
    public void Caoch_Constructor()
    {
        var coach = new Coach("James Bond", "email@mail.com");

        Assert.Equal("James Bond", coach.Name);
        Assert.Equal("email@mail.com", coach.Email);
        Assert.NotNull(coach.Skills);
        Assert.NotNull(coach.AssignedCourses);

        Assert.Empty(coach.Skills);
        Assert.Empty(coach.AssignedCourses);
    }

    [Fact]
    public void AddSkills()
    {
        var coach = new Coach("Makaka", "Email@mail.com");

        // Add skill
        coach.AddSkill("Dancing");
        Assert.Contains("dancing", coach.Skills);

        // Add same skill
        var addSameSkill = Assert.Throws<ArgumentException>(() => coach.AddSkill("Dancing"));
        Assert.Equal("Skill already added", addSameSkill.Message);

    }

    [Fact]
    public void UpdateSkills_ShouldReplaceAllSkills()
    {
        var coach = new Coach("John", "john@example.com");
        coach.AddSkill("C#");

        coach.UpdateSkills(new List<string> { "Java", "Python" });

        Assert.DoesNotContain("C#", coach.Skills);
        Assert.Contains("Java", coach.Skills);
        Assert.Contains("Python", coach.Skills);
        Assert.Equal(2, coach.Skills.Count);
    }

    [Fact]
    public void HasAllSkills_ShouldCheckSkills()
    {
        Coach coach = new("Ban", "email");
        coach.AddSkill("Jumping");
        coach.AddSkill("Running");

        Assert.True(coach.HasAllSkills(["Jumping"]));
        Assert.True(coach.HasAllSkills(["jumping", "running"]));
        Assert.False(coach.HasAllSkills(["jumping", "running", "walking"]));
    }

    [Fact]
    public void CoachAssignCourses_WithException()
    {
        Coach coach = new("Ban", "email");

        var start = new DateOnly(2025, 5, 1);
        var end = new DateOnly(2025, 6, 1);
        var course = new Course("Math", new TimeDay(start, end));

        coach.AssignCourse(course);

        Assert.Contains(course, coach.AssignedCourses);

        var exc = Assert.Throws<ArgumentException>(() => coach.AssignCourse(course));
        Assert.Equal("Course is already assigned", exc.Message);
    }

    [Fact]
    public void AreTimeSlotsOverlapping_withOtherTimeSlots()
    {
        Coach coach = new("Ban", "email");

        DateOnly start = new(2025, 5, 1);
        DateOnly end = new(2025, 6, 1);


        TimeSlot time1 = new(WeekDay.Monday, 10, 12);
        TimeSlot time2 = new(WeekDay.Monday, 10, 17);

        Course course = new("Math", new TimeDay(start, end));
        Course course1 = new("NotMath", new TimeDay(start, end));
        course.AddTimeSlot(time1);
        course1.AddTimeSlot(time2);


        coach.AssignCourse(course);

        var exception = Assert.Throws<ArgumentException>(() => coach.AssignCourse(course1));
        Assert.Equal("Lesson time is overlapping", exception.Message);
    }
}