using Microsoft.AspNetCore.Components;
using HorsesForCourses.Blazor.Dtos;

namespace HorsesForCourses.Blazor.Pages.CoachComponents
{
    public partial class CoachDetails
    {
        [Parameter]
        public CoachDetailsDto? Details { get; set; }
    }
}
