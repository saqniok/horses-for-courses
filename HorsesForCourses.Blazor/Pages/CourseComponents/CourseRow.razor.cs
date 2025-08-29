using Microsoft.AspNetCore.Components;
using HorsesForCourses.Blazor.Dtos;

namespace HorsesForCourses.Blazor.Pages.CourseComponents
{
    public partial class CourseRow
    {
        [Parameter]
        public CourseDto? Course { get; set; }

        [Parameter]
        public EventCallback<int> OnShowDeleteConfirm { get; set; }

        [Parameter]
        public EventCallback<int> OnConfirm { get; set; }

        [Parameter]
        public EventCallback<int> OnEdit { get; set; }

        [Parameter]
        public EventCallback<int> OnAssignCoach { get; set; }

        [Parameter]
        public bool ShowDetails { get; set; }

        [Parameter]
        public EventCallback OnToggleDetails { get; set; }

        private async Task ToggleDetails()
        {
            await OnToggleDetails.InvokeAsync();
        }
    }
}