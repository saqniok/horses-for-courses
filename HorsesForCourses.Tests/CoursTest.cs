using HorsesForCourses.Core;

namespace HorsesForCourses.Test;


public class CourseTests
{

    [Fact]
    public void Confirm_ShouldThrow_WhenNoLessons()
    {
        var start = new DateOnly(2025, 8, 1);
        var end = new DateOnly(2025, 8, 31);
        var period = new TimeDay(start, end);
        var course = new Course("Math", period);

        var ex = Assert.Throws<InvalidOperationException>(() => course.Confirm());
        Assert.Equal("Cannot confirm course without any lessons.", ex.Message);
    }

    [Fact]
    public void ConfimedChecker()
    {
        var start = new DateOnly(2025, 8, 1);
        var end = new DateOnly(2025, 8, 31);
        var period = new TimeDay(start, end);
        var course = new Course("Math", period);

        course.AddTimeSlot(new TimeSlot(WeekDay.Monday, 10, 12));
        course.Confirm();

        Assert.True(course.IsConfirmed);
    }

    [Fact]
    public void AddAndRemoveSkillToCourse_AndAssingedCoach()
    {
        var coach = new Coach("James Bond", "email@mail.com");
        coach.AddSkill("Physics");

        var start = new DateOnly(2025, 8, 1);
        var end = new DateOnly(2025, 8, 31);
        var period = new TimeDay(start, end);
        var course = new Course("Math", period);

        course.AddRequiredSkill("Physics");

        Assert.Contains("physics", course.RequiredSkills);
        Assert.True(coach.HasAllSkills(course.RequiredSkills));

        course.AddRequiredSkill("math");
        Assert.False(coach.HasAllSkills(course.RequiredSkills));
    }

    [Fact]
    public void AssignCoach_ShouldThrow_IfNotConfirmed()
    {
        var start = new DateOnly(2025, 8, 1);
        var end = new DateOnly(2025, 8, 31);
        var period = new TimeDay(start, end);
        var course = new Course("Math", period);
        course.AddRequiredSkill("Math");

        var coach = new Coach("Alice", "alice@email.com");

        var ex = Assert.Throws<InvalidOperationException>(() => course.AssignCoach(coach));
        Assert.Equal("Course must be confirmed before assigning a coach.", ex.Message);
    }

    [Fact]
    public void AssignCoach_ShouldThrow_IfCoachHasNotAllSkills()
    {
        var start = new DateOnly(2025, 8, 1);
        var end = new DateOnly(2025, 8, 31);
        var period = new TimeDay(start, end);
        var course = new Course("Math", period);
        course.AddRequiredSkill("Math");
        course.AddTimeSlot(new TimeSlot(WeekDay.Friday, 9, 12));
        course.Confirm();

        var coach = new Coach("Alice", "alice@email.com");

        var ex = Assert.Throws<InvalidOperationException>(() => course.AssignCoach(coach));
        Assert.Equal("Coach does not have all required skills.", ex.Message);
    }

    [Fact]
    public void AssignCoach_ShoudlAssingCoach()
    {
        var start = new DateOnly(2025, 8, 1);
        var end = new DateOnly(2025, 8, 31);
        var period = new TimeDay(start, end);
        var course = new Course("Math", period);
        course.AddRequiredSkill("Math");
        course.AddTimeSlot(new TimeSlot(WeekDay.Friday, 9, 12));
        course.Confirm();

        var coach = new Coach("Alice", "alice@email.com");
        coach.AddSkill("math");

        course.AssignCoach(coach);

    }
}