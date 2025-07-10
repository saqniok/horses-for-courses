namespace HorsesForCourses.Core;


public class Coach
{
    public string Name { get; }
    public List<string> Skills { get; }

    public Coach(string name, List<string> skills)
    {
        Name = name;            // ?? throw new ArgumentNullException(nameof(name));
        Skills = skills;        // ?? throw new ArgumentNullException(nameof(skills));
    }

    public void AddSkill(string skill) => Skills.Add(skill);
    public void RemoveSkill(string skill) => Skills.Remove(skill);
    public bool HasAllSkills(IEnumerable<string> requiredSkills)
        => requiredSkills.All(skill => Skills.Contains(skill));


}


