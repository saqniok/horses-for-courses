using HorsesForCourses.Blazor.Dtos;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace HorsesForCourses.Blazor.Pages.Components
{
    public partial class AddCoachModal
    {
        [Parameter]
        public bool Show { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback<CreateCoachRequest> OnConfirm { get; set; }

        private CreateCoachRequest NewCoach { get; set; } = new() { Name = string.Empty, Email = string.Empty };

        private async Task HandleConfirm()
        {
            await OnConfirm.InvokeAsync(NewCoach);
            NewCoach = new() { Name = string.Empty, Email = string.Empty };
        }
    }
}