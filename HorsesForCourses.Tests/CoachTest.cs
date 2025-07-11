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
    public void AddAndRemoveSkills()
    {
        var coach = new Coach("Makaka", "Email@mail.com");

        // Add skill
        coach.AddSkill("Dancing");
        Assert.Contains("dancing", coach.Skills);

        // Add same skill
        var addSameSkill = Assert.Throws<ArgumentException>(() => coach.AddSkill("Dancing"));
        Assert.Equal("Skill already added", addSameSkill.Message);

        // Remove skill
        coach.RemoveSkill("Dancing");
        Assert.Empty(coach.Skills);

        // Remove wrong skill
        var wrongSkill = Assert.Throws<ArgumentException>(() => coach.RemoveSkill("Sing"));
        Assert.Equal("There is no skill in list", wrongSkill.Message);
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

        var start = new DateTime(2025, 5, 1);
        var end = new DateTime(2025, 6, 1);
        var course = new Course("Math", new Period(start, end));

        coach.AssignCourse(course);

        Assert.Contains(course, coach.AssignedCourses);

        var exc = Assert.Throws<ArgumentException>(() => coach.AssignCourse(course));
        Assert.Equal("Course is already assinged", exc.Message);
    }

    [Fact]
    public void AreTimeSlotsOverlapping_withOtherTimeSlots()
    {
        Coach coach = new("Ban", "email");

        DateTime start = new(2025, 5, 1);
        DateTime end = new(2025, 6, 1);


        TimeSlot time1 = new(WeekDay.Monday, 10, 12);
        TimeSlot time2 = new(WeekDay.Monday, 10, 17);
        TimeSlot time3 = new(WeekDay.Friday, 15, 17);

        Course course = new("Math", new Period(start, end));
        course.AddTimeSlot(time1);
        course.AddTimeSlot(time2);

        coach.AssignCourse(course);

        var exception = Assert.Throws<ArgumentException>(() => coach.IsAvailableCoach());
        Assert.Equal("Lesson time is overlapping", exception.Message);

        //Remove OverlapingTime and add another
        course.RemoveTimeSlot(time2);
        course.AddTimeSlot(time3);

        Assert.True(coach.IsAvailableCoach());
    }
}