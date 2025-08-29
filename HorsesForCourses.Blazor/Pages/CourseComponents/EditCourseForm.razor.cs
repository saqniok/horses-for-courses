using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Core;

namespace HorsesForCourses.Blazor.Pages.CourseComponents
{
    public partial class EditCourseForm
    {
        [Parameter]
        public CourseDto? Course { get; set; }

        [Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        private string newSkill = string.Empty;
        private TimeSlotDto newTimeSlot = new TimeSlotDto { Day = WeekDay.Monday, Start = 9, End = 10 };

        private void AddSkill()
        {
            if (!string.IsNullOrWhiteSpace(newSkill) && Course != null)
            {
                var skills = Course.RequiredSkills?.ToList() ?? new List<string>();
                if (!skills.Contains(newSkill.Trim()))
                {
                    skills.Add(newSkill.Trim());
                    Course.RequiredSkills = skills.AsReadOnly();
                    newSkill = string.Empty;
                }
            }
        }

        private Task AddSkillOnEnter(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                AddSkill();
            }
            return Task.CompletedTask;
        }

        private void RemoveSkill(string skill)
        {
            if (Course?.RequiredSkills != null)
            {
                var skills = Course.RequiredSkills.ToList();
                skills.Remove(skill);
                Course.RequiredSkills = skills.AsReadOnly();
            }
        }

        private void AddTimeSlot()
        {
            if (Course != null && newTimeSlot.Start < newTimeSlot.End)
            {
                var schedule = Course.Schedule?.ToList() ?? new List<TimeSlotDto>();

                // Check if this time slot already exists
                bool exists = schedule.Any(slot =>
                    slot.Day == newTimeSlot.Day &&
                    slot.Start == newTimeSlot.Start &&
                    slot.End == newTimeSlot.End);

                if (!exists)
                {
                    schedule.Add(new TimeSlotDto
                    {
                        Day = newTimeSlot.Day,
                        Start = newTimeSlot.Start,
                        End = newTimeSlot.End
                    });
                    Course.Schedule = schedule.AsReadOnly();

                    // Reset form
                    newTimeSlot = new TimeSlotDto { Day = WeekDay.Monday, Start = 9, End = 10 };
                }
            }
        }

        private void RemoveTimeSlot(TimeSlotDto timeSlot)
        {
            if (Course?.Schedule != null)
            {
                var schedule = Course.Schedule.ToList();
                var toRemove = schedule.FirstOrDefault(ts => ts.Day == timeSlot.Day &&
                                                            ts.Start == timeSlot.Start &&
                                                            ts.End == timeSlot.End);
                if (toRemove != null)
                {
                    schedule.Remove(toRemove);
                    Course.Schedule = schedule.AsReadOnly();
                }
            }
        }

        private string FormatTime(int hour)
        {
            return $"{hour:D2}:00";
        }
    }
}