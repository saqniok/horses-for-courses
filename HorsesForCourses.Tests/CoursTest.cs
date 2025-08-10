

using HorsesForCourses.Core;

public class CourseTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var period = new TimeDay(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(7)));
        var course = new Course("Test Course", period);

        Assert.Equal("Test Course", course.Title);
        Assert.Equal(period, course.Period);
        Assert.Empty(course.RequiredSkills);
        Assert.Empty(course.Schedule);
        Assert.False(course.IsConfirmed);
        Assert.Null(course.AssignedCoach);
    }

    [Fact]
    public void Confirm_ShouldSetIsConfirmed_WhenScheduleIsNotEmpty()
    {
        var period = new TimeDay(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(7)));
        var course = new Course("Test", period);

        course.AddTimeSlot(new TimeSlot(WeekDay.Monday, 10, 11));

        course.Confirm();

        Assert.True(course.IsConfirmed);
    }

    [Fact]
    public void Confirm_ShouldThrow_WhenScheduleIsEmpty()
    {
        var period = new TimeDay(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(7)));
        var course = new Course("Test", period);

        var ex = Assert.Throws<InvalidOperationException>(() => course.Confirm());
        Assert.Equal("Cannot confirm course without any lessons.", ex.Message);
    }

    [Fact]
    public void AddRequiredSkill_ShouldAddSkill_WhenNotConfirmed()
    {
        var course = CreateCourse();

        course.AddRequiredSkill("C#");

        Assert.Contains("C#", course.RequiredSkills);
    }

    [Fact]
    public void AddRequiredSkill_ShouldThrow_WhenConfirmed()
    {
        var course = CreateConfirmedCourse();

        var ex = Assert.Throws<InvalidOperationException>(() => course.AddRequiredSkill("C#"));
        Assert.Equal("Cannot modify course after it has been confirmed.", ex.Message);
    }

    [Fact]
    public void RemoveRequiredSkill_ShouldRemoveSkill_WhenNotConfirmed()
    {
        var course = CreateCourse();
        course.AddRequiredSkill("C#");

        course.RemoveRequiredSkill("C#");

        Assert.DoesNotContain("C#", course.RequiredSkills);
    }

    [Fact]
    public void RemoveRequiredSkill_ShouldThrow_WhenConfirmed()
    {
        var course = CreateConfirmedCourse();

        var ex = Assert.Throws<InvalidOperationException>(() => course.RemoveRequiredSkill("C#"));
        Assert.Equal("Cannot modify course after it has been confirmed.", ex.Message);
    }

    [Fact]
    public void UpdateRequiredSkills_ShouldReplaceSkills_WhenNotConfirmed()
    {
        var course = CreateCourse();
        course.AddRequiredSkill("C#");

        course.UpdateRequiredSkills(new[] { "Java", "Python" });

        Assert.DoesNotContain("C#", course.RequiredSkills);
        Assert.Contains("Java", course.RequiredSkills);
        Assert.Contains("Python", course.RequiredSkills);
    }

    [Fact]
    public void UpdateRequiredSkills_ShouldThrow_WhenConfirmed()
    {
        var course = CreateConfirmedCourse();

        var ex = Assert.Throws<InvalidOperationException>(() => course.UpdateRequiredSkills(new[] { "C#" }));
        Assert.Equal("Cannot modify course after it has been confirmed.", ex.Message);
    }

    [Fact]
    public void AddTimeSlot_ShouldAdd_WhenNotConfirmed()
    {
        var course = CreateCourse();

        var slot = new TimeSlot(WeekDay.Monday, 9, 10);
        course.AddTimeSlot(slot);

        Assert.Contains(slot, course.Schedule);
    }

    [Fact]
    public void AddTimeSlot_ShouldThrow_WhenConfirmed()
    {
        var course = CreateConfirmedCourse();

        var slot = new TimeSlot(WeekDay.Monday, 9, 10);
        var ex = Assert.Throws<InvalidOperationException>(() => course.AddTimeSlot(slot));
        Assert.Equal("Cannot modify course after it has been confirmed.", ex.Message);
    }

    [Fact]
    public void AssignCoach_ShouldAssign_WhenConfirmedAndCoachHasSkills()
    {
        var course = CreateCourse();
        course.UpdateRequiredSkills(["C#"]);
        var slot = new TimeSlot(WeekDay.Monday, 9, 10);
        course.AddTimeSlot(slot);
        course.UpdateRequiredSkills(new[] { "C#" });

        var coach = new Coach("John", "john@example.com");
        coach.UpdateSkills(new[] { "C#" });

        course.Confirm();
        course.AssignCoach(coach);

        Assert.Equal(coach, course.AssignedCoach);
        Assert.Contains(course, coach.AssignedCourses);
    }

    [Fact]
    public void AssignCoach_ShouldThrow_WhenNotConfirmed()
    {
        var course = CreateCourse();
        var coach = new Coach("John", "john@example.com");

        var ex = Assert.Throws<InvalidOperationException>(() => course.AssignCoach(coach));
        Assert.Equal("Course must be confirmed before assigning a coach.", ex.Message);
    }

    [Fact]
    public void AssignCoach_ShouldThrow_WhenCoachLacksSkills()
    {
        var course = CreateCourse();
        course.UpdateRequiredSkills(["C#"]);
        var slot = new TimeSlot(WeekDay.Monday, 9, 10);
        course.AddTimeSlot(slot);

        var coach = new Coach("John", "john@example.com");
        coach.UpdateSkills(new[] { "Java" });

        course.Confirm();
        var ex = Assert.Throws<InvalidOperationException>(() => course.AssignCoach(coach));
        Assert.Equal("Coach does not have all required skills.", ex.Message);
    }


    // Вспомогательные методы
    private Course CreateCourse()
    {
        var period = new TimeDay(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(7)));
        return new Course("Test Course", period);
    }

    private Course CreateConfirmedCourse()
    {
        var course = CreateCourse();
        course.AddTimeSlot(new TimeSlot(WeekDay.Monday, 10, 11));
        course.Confirm();
        return course;
    }
}
