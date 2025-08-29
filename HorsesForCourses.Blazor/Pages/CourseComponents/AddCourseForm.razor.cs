using Microsoft.AspNetCore.Components;
using HorsesForCourses.Blazor.Dtos;

namespace HorsesForCourses.Blazor.Pages.CourseComponents
{
    public partial class AddCourseForm
    {
        [Parameter]
        public bool Show { get; set; }

        [Parameter]
        public CreateCourseRequestDto Course { get; set; } = new CreateCourseRequestDto { Title = string.Empty };

        [Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        private async Task HandleValidSubmit()
        {
            await OnValidSubmit.InvokeAsync();
        }

        private void HandleInvalidSubmit()
        {
            // Form has validation errors, do nothing as ValidationSummary and ValidationMessage will display them
        }
    }
}