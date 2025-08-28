using Microsoft.AspNetCore.Components;
using HorsesForCourses.Blazor.Dtos;

namespace HorsesForCourses.Blazor.Pages.CoachComponents
{
    // This is the class definition for the `CoachDetails` component. 
    // The keyword `partial` indicates that this class is part of a larger class, 
    // which is also defined in a .razor file. Blazor combines them during compilation
    public partial class CoachDetails
    {
        [Parameter] // This attribute indicates that the `Details` property can be set by the parent component. That is, the parent component that uses `CoachDetails` passes an object with coach details to it
        public CoachDetailsDto? Details { get; set; }
    }
}
