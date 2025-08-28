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
        public ICoachService CoachService { get; set; } = default!;
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;

        protected List<CourseDto>? courses;
        protected List<CoachSummaryResponse>? availableCoaches;
        protected string? error;

        // fields for AddCourseForm modal
        private bool showAddCourseModal = false;
        private CreateCourseRequestDto newCourse = new CreateCourseRequestDto { Title = string.Empty };

        // fields for EditCourseForm modal
        private bool showEditCourseModal = false;
        private CourseDto? editingCourse;

        // fields for AssignCoachForm modal
        private bool showAssignCoachModal = false;
        private CourseDto? assigningCourse;
        private List<CoachDetailsDto>? eligibleCoaches;

        protected override async Task OnInitializedAsync()
        {
            await LoadCourses();
            await LoadCoaches();
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

        protected async Task LoadCoaches()
        {
            try
            {
                availableCoaches = await CoachService.GetCoachesAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        // methods for AddCourseForm modal
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

        // methods for EditCourseForm modal
        private void ShowEditCourseModal(int courseId)
        {
            editingCourse = courses?.FirstOrDefault(c => c.Id == courseId);
            if (editingCourse != null)
            {
                showEditCourseModal = true;
            }
        }

        private async Task UpdateCourse()
        {
            if (editingCourse != null)
            {
                try
                {
                    await CourseService.UpdateCourseAsync(editingCourse.Id, editingCourse);
                    showEditCourseModal = false;
                    await LoadCourses(); // Refresh the list
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
        }

        private void HideEditCourseModal()
        {
            showEditCourseModal = false;
        }

        // methods for AssignCoachForm modal
        private async void ShowAssignCoachModal(int courseId)
        {
            assigningCourse = courses?.FirstOrDefault(c => c.Id == courseId);
            if (assigningCourse != null)
            {
                await LoadEligibleCoaches();
                showAssignCoachModal = true;
                StateHasChanged();
            }
        }

        private async Task LoadEligibleCoaches()
        {
            if (availableCoaches != null)
            {
                eligibleCoaches = new List<CoachDetailsDto>();
                foreach (var coach in availableCoaches)
                {
                    try
                    {
                        var detailedCoach = await CoachService.GetCoachDetailsAsync(coach.Id);
                        eligibleCoaches.Add(detailedCoach);
                    }
                    catch (Exception ex)
                    {
                        error = $"Error loading coach details: {ex.Message}";
                    }
                }
            }
        }

        private void HideAssignCoachModal()
        {
            showAssignCoachModal = false;
            assigningCourse = null;
            eligibleCoaches = null;
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

        // methods for coach assignment
        private async Task AssignCoachToCourse(int coachId)
        {
            if (assigningCourse != null)
            {
                try
                {
                    await CourseService.AssignCoachAsync(assigningCourse.Id, coachId);
                    showAssignCoachModal = false;
                    await LoadCourses(); // Refresh the list to get updated coach info
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
        }

        private Task UnassignCoachFromCourse()
        {
            if (editingCourse != null && editingCourse.Coach != null)
            {
                try
                {
                    // Note: The API doesn't have an unassign endpoint, so we would need to add one
                    // For now, we'll show an error message
                    error = "Unassign coach functionality needs to be implemented in the API";
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            return Task.CompletedTask;
        }
    }
}
