using Microsoft.AspNetCore.Components;
using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Core;

namespace HorsesForCourses.Blazor.Pages.CourseComponents
{
    public partial class CourseDetailsExpanded
    {
        [Parameter]
        public CourseDto? Course { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        private bool IsHourScheduled(WeekDay day, int hour)
        {
            if (Course?.Schedule == null)
                return false;

            return Course.Schedule.Any(slot =>
                slot.Day == day &&
                hour >= slot.Start &&
                hour <= slot.End);
        }
    }
}