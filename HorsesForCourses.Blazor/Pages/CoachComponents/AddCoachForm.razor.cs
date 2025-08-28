using Microsoft.AspNetCore.Components;
using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Blazor.Services;

namespace HorsesForCourses.Blazor.Pages.CoachComponents
{
    public partial class AddCoachForm
    {
        [Inject]
        private ICoachService? CoachService { get; set; }

        [Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        private CreateCoachRequest newCoach = new() { Name = string.Empty, Email = string.Empty };
        private string? error;

        private async Task HandleValidSubmit()
        {
            try
            {
                await CoachService!.AddCoachAsync(newCoach);
                await OnValidSubmit.InvokeAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private void HandleClose()
        {
            OnClose.InvokeAsync();
        }
    }
}
