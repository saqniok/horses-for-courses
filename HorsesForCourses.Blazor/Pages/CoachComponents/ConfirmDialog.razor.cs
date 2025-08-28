using Microsoft.AspNetCore.Components;

namespace HorsesForCourses.Blazor.Pages.CoachComponents
{
    public partial class ConfirmDialog
    {
        [Parameter]     // [Parameter] makes this property accessible to the parent component, which can pass a value when using this component
        public string Title { get; set; } = "Confirm";

        [Parameter]     // [Parameter] makes this property accessible to the parent component, which can pass a value when using this component
        public string Message { get; set; } = "Are you sure?";

        [Parameter]     // [Parameter] makes this property accessible to the parent component, which can pass a value when using this component
        public EventCallback OnConfirm { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        private void Confirm()
        {
            OnConfirm.InvokeAsync();
            // This is the key line. It calls the `OnConfirm` event callback asynchronously. 
            // This allows the parent component to be notified that the action has been confirmed, 
            // and the parent can run its logic (for example, delete the object from the database)
        }

        private void Cancel()
        {
            OnCancel.InvokeAsync();
            // calls the `OnCancel` event callback, 
            // notifying the parent that the action has been canceled. 
            // The parent component can then close the dialog box without performing any other actions
        }
    }
}
