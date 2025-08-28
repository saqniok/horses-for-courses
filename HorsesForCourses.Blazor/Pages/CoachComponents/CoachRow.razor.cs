using Microsoft.AspNetCore.Components;
using HorsesForCourses.Blazor.Dtos;

namespace HorsesForCourses.Blazor.Pages.CoachComponents
{
    public partial class CoachRow
    {
        [Parameter]
        public CoachSummaryResponse? Coach { get; set; }

        [Parameter]
        public EventCallback<int> OnEdit { get; set; }

        [Parameter]
        public EventCallback<int> OnDelete { get; set; }

        [Parameter]
        public EventCallback<int> OnToggleDetails { get; set; }

        private void Edit()
        {
            if (Coach != null) OnEdit.InvokeAsync(Coach.Id);
        }

        private void Delete()
        {
            if (Coach != null) OnDelete.InvokeAsync(Coach.Id);
        }

        private void ToggleDetails()
        {
            if (Coach != null) OnToggleDetails.InvokeAsync(Coach.Id);
        }
    }
}
