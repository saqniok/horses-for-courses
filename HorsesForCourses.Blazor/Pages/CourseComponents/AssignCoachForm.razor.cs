using Microsoft.AspNetCore.Components;
using HorsesForCourses.Blazor.Dtos;

namespace HorsesForCourses.Blazor.Pages.CourseComponents
{
    public partial class AssignCoachForm
    {
        [Parameter]
        public CourseDto? Course { get; set; }

        [Parameter]
        public List<CoachDetailsDto>? EligibleCoaches { get; set; }

        [Parameter]
        public EventCallback<int> OnAssignCoach { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        private int selectedCoachId = 0;

        private async Task AssignCoach()
        {
            if (selectedCoachId > 0)
            {
                await OnAssignCoach.InvokeAsync(selectedCoachId);
            }
        }
    }
}