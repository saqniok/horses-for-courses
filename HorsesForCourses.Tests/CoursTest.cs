using HorsesForCourses.Core;

namespace HorsesForCourses.Test;

public class CourseTests
{
    [Fact]
    public void Confirm_ShouldThrow_WhenNoLessons()
    {
        var period = new Period(DateTime.Today, DateTime.Today.AddDays(30));
        var course = new Course("Math", period);

        var ex = Assert.Throws<InvalidOperationException>(() => course.Confirm());
        Assert.Equal("Cannot confirm course without any lessons.", ex.Message);
    }

    [Fact]
    public void ConfimedChecker()
    {
        var period = new Period(DateTime.Today, DateTime.Today.AddDays(30));
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

        var period = new Period(DateTime.Today, DateTime.Today.AddDays(30));
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
        var period = new Period(DateTime.Today, DateTime.Today.AddDays(30));
        var course = new Course("Math", period);
        course.AddRequiredSkill("Math");

        var coach = new Coach("Alice", "alice@email.com");

        var ex = Assert.Throws<InvalidOperationException>(() => course.AssignCoach(coach));
        Assert.Equal("Course must be confirmed before assigning a coach.", ex.Message);
    }

    [Fact]
    public void AssignCoach_ShouldThrow_IfCoachHasNotAllSkills()
    {
        var period = new Period(DateTime.Today, DateTime.Today.AddDays(30));
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
        var period = new Period(DateTime.Today, DateTime.Today.AddDays(30));
        var course = new Course("Math", period);
        course.AddRequiredSkill("Math");
        course.AddTimeSlot(new TimeSlot(WeekDay.Friday, 9, 12));
        course.Confirm();

        var coach = new Coach("Alice", "alice@email.com");
        coach.AddSkill("math");

        course.AssignCoach(coach);
        
    }
}