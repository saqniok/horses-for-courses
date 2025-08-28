namespace HorsesForCourses.Blazor.Pages.CoachComponents
{
    public partial class CoachRow
    {
        // Parameter to receive the coach data from the parent component
        [Parameter]
        public CoachSummaryResponse? Coach { get; set; }

        // EventCallback to notify the parent component when the "Edit" button is clicked
        // It passes the coach's ID as an integer.
        [Parameter]
        public EventCallback<int> OnEdit { get; set; }

        // EventCallback to notify the parent component when the "Delete" button is clicked
        public EventCallback<int> OnDelete { get; set; }

        // EventCallback to notify the parent component when the "Toggle Details" button is clicked
        public EventCallback<int> OnToggleDetails { get; set; }

        // Method called when the "Edit" action is triggered in the UI
        private void Edit()
        {
            // Check if the Coach object is not null before invoking the callback
            if (Coach != null) OnEdit.InvokeAsync(Coach.Id);
        }

        // Method called when the "Delete" action is triggered
        private void Delete()
        {
            // Invoke the OnDelete callback with the coach's ID
            if (Coach != null) OnDelete.InvokeAsync(Coach.Id);
        }

        // Method called when the "Toggle Details" action is triggered
        private void ToggleDetails()
        {
            // Invoke the OnToggleDetails callback with the coach's ID
            if (Coach != null) OnToggleDetails.InvokeAsync(Coach.Id);
        }
    }
}