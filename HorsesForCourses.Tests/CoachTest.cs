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
        


    }
}