using HorsesForCourses.Blazor.Services;
using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Blazor.Pages.CourseComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HorsesForCourses.Blazor.Pages
{
    public partial class Courses : ComponentBase
    {
        [Inject]
        public ICourseService CourseService { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        protected List<CourseDto>? courses;
        protected string? error;

        // New fields for AddCourseForm modal
        private bool showAddCourseModal = false;
        private CreateCourseRequestDto newCourse = new CreateCourseRequestDto { Title = string.Empty };

        protected override async Task OnInitializedAsync()
        {
            await LoadCourses();
        }

        protected async Task LoadCourses()
        {
            try
            {
                courses = await CourseService.GetCoursesAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            StateHasChanged();
        }

        // New methods for AddCourseForm modal
        private void ShowAddCourseModal()
        {
            showAddCourseModal = true;
            newCourse = new CreateCourseRequestDto { Title = string.Empty, StartDate = DateOnly.FromDateTime(DateTime.Today), EndDate = DateOnly.FromDateTime(DateTime.Today) }; // Initialize for new entry
        }

        private async Task HandleAddCourseValidSubmit()
        {
            try
            {
                await CourseService.AddCourseAsync(newCourse);
                showAddCourseModal = false;
                await LoadCourses(); // Refresh the list
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private void HideAddCourseModal()
        {
            showAddCourseModal = false;
        }

        protected async Task DeleteCourse(int id)
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this course?");
            if (confirmed)
            {
                try
                {
                    await CourseService.DeleteCourseAsync(id);
                    await LoadCourses(); // Refresh the list
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
        }

        protected async Task ConfirmCourse(int id)
        {
            try
            {
                await CourseService.ConfirmCourseAsync(id);
                await LoadCourses(); // Refresh the list
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }
    }
}