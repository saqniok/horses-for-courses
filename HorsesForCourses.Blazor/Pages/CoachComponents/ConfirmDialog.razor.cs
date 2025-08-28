using Microsoft.AspNetCore.Components;

namespace HorsesForCourses.Blazor.Pages.CoachComponents
{
    public partial class ConfirmDialog
    {
        [Parameter]
        public string Title { get; set; } = "Confirm";

        [Parameter]
        public string Message { get; set; } = "Are you sure?";

        [Parameter]
        public EventCallback OnConfirm { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        private void Confirm()
        {
            OnConfirm.InvokeAsync();
        }

        private void Cancel()
        {
            OnCancel.InvokeAsync();
        }
    }
}
