using Microsoft.AspNetCore.Components;
using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Blazor.Services;

namespace HorsesForCourses.Blazor.Pages.CoachComponents
{
    /*
        The keyword `partial` indicates that this class is part of another class. 
        This is standard practice in Blazor, where one file (.razor) 
        contains the UI markup and another (.razor.cs or simply .cs) contains the logic. 
        They are compiled into a single class.
    */
    public partial class AddCoachForm
    {
        // This code uses Dependency Injection (DI).

        // [Inject] is an attribute that tells Blazor to inject an instance of the `ICoachService` service. 
        // Blazor searches the DI container for a registered service that 
        // implements the `ICoachService` interface and automatically assigns it to this property.
        [Inject]             
        private ICoachService? CoachService { get; set; }


        [Parameter]     // [Parameter] — an attribute that allows a parent component to pass data or callback functions to a child component.
        // `EventCallback` is a special type in Blazor designed for event handling. It allows a child component to call a method in the parent component
        public EventCallback OnValidSubmit { get; set; }    // OnValidSubmit will be called when the form is successfully submitted.
        

        [Parameter]     // [Parameter] — an attribute that allows a parent component to pass data or callback functions to a child component.
        // `EventCallback` is a special type in Blazor designed for event handling. It allows a child component to call a method in the parent component
        public EventCallback OnClose { get; set; }  // OnClose — when the modal window should be closed


        // This field stores data for the new form. It is initialized with empty values. This object is linked to the form in Razor markup via Model="@newCoach"
        private CreateCoachRequest newCoach = new() { Name = string.Empty, Email = string.Empty };

        
        //This field stores an error message if one occurs. The ? symbol indicates that the field can be null (nullable). This is the field used for conditional rendering of the error message
        private string? error;


        // This method is called when the form successfully passes client-side validation and is submitted
        private async Task HandleValidSubmit()
        {
            // Standard try-catch block for error handling. If the `AddCoachAsync` service call ends with an error, it will be caught
            try
            {
                // Async call to the `AddCoachAsync` method of the DI service. 
                // It sends the new coach's data to the server. 
                // The exclamation mark ! is a null-forgiving operator 
                // that tells the compiler that CoachService will not be null at this point, 
                // as it has been injected
                await CoachService!.AddCoachAsync(newCoach);

                // If `AddCoachAsync` completes successfully, 
                // this code calls EventCallback OnValidSubmit. 
                // This notifies the parent component that the data has been successfully saved, 
                // and the parent can, for example, update the list of coaches or close the modal window
                await OnValidSubmit.InvokeAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        
        // This method is called when the user clicks the close button. 
        // It calls EventCallback OnClose, informing the parent component that the modal window should be closed
        private void HandleClose()
        {
            OnClose.InvokeAsync();
        }
    }
}
