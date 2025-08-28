using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Blazor.Services;

namespace HorsesForCourses.Blazor.Pages.CoachComponents
{
    public partial class EditCoachForm
    {
        [Inject]
        private ICoachService? CoachService { get; set; }
        // This attribute uses `Dependency Injection`
        // to obtain an instance of the `ICoachService` service, 
        // which is used to interact with the backend (for example, to update data)

        [Parameter]
        public CoachDetailsDto? EditingCoach { get; set; }
        // This parameter receives a DTO object from the parent component. 
        // This object contains trainer data that will be displayed and edited in the form. 
        // The ? symbol makes it nullable, which means that the data may not have been loaded yet

        [Parameter]
        public EventCallback OnCoachUpdated { get; set; }       // These parameters are EventCallbacks. 
                                                                // They allow the child component to notify the parent 
        [Parameter]                                             // that the data has been successfully updated or 
        public EventCallback OnCancel { get; set; }             // that the user has canceled the action



        private string newSkill = string.Empty;
        private string? error;


        // This method is called when the user submits a form to update data
        private async Task UpdateCoach()
        {
            // Safety check: ensure the EditingCoach object is not null. 
            // This prevents a NullReferenceException if the method is called before data is loaded.
            if (EditingCoach == null) return;

            try
            {
                // 1. Update the main coach details.
                // It awaits the asynchronous call to the service method, passing the coach's ID and the updated DTO.
                await CoachService!.UpdateCoachAsync(EditingCoach.Id, EditingCoach);

                // 2. Update the coach's skills.
                // This is a separate call, indicating that skills might be managed by a different API endpoint.
                // It creates a new DTO specifically for updating skills.
                await CoachService!.UpdateCoachSkillsAsync(EditingCoach.Id, new UpdateCoachSkillsDto { Skills = EditingCoach.Skills });

                // 3. Notify the parent component.
                // If both updates are successful, it invokes the EventCallback to notify the parent
                // that the coach data has been updated. The parent can then refresh its UI or close the form.
                await OnCoachUpdated.InvokeAsync();
            }
            catch (Exception ex)
            {
                // Error handling.
                // If any of the above API calls fail, the exception is caught, and the error message
                // is stored in the 'error' field, which will be displayed to the user in the UI.
                error = ex.Message;
            }
        }

        private void AddSkill()
        {
            // Conditional check to ensure the new skill is valid before adding it to the list.
            if (
                EditingCoach != null &&                 // 1. Ensures the main coach object is not null.
                !string.IsNullOrWhiteSpace(newSkill) && // 2. Ensures the new skill string is not empty or just whitespace.
                !EditingCoach.Skills.Contains(newSkill) // 3. Prevents adding duplicate skills.
            )
            {
                // If all conditions are met, the code inside this block will execute.
                
                // Adds the new skill to the local list. This immediately updates the UI due to Blazor's data binding.
                EditingCoach.Skills.Add(newSkill);
                
                // Resets the input field to an empty string, ready for the next entry.
                newSkill = string.Empty;
            }
        }

        private void AddSkillOnEnter(KeyboardEventArgs e)
        {
            // Check if the pressed key is either the 'Enter' or 'NumpadEnter' key.
            // This allows the user to add a skill by pressing Enter in the input field.
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                // If the key is 'Enter', call the existing AddSkill() method to handle the logic
                // of adding the skill to the list and clearing the input field.
                AddSkill();
            }
        }

        private async Task RemoveSkill(string skillToRemove)
        {
            // Safety check: exit the method if the coach object is null to prevent errors.
            if (EditingCoach == null) return;

            try
            {
                // Asynchronously call the service to remove the skill from the database.
                await CoachService!.RemoveCoachSkillAsync(EditingCoach.Id, skillToRemove);

                // If the service call is successful, remove the skill from the local collection.
                // This updates the UI and keeps the client-side state in sync with the server.
                EditingCoach.Skills.Remove(skillToRemove);
            }
            catch (Exception ex)
            {
                // If an error occurs during the API call, catch the exception.
                // Store the error message to be displayed to the user in the UI.
                error = ex.Message;
            }
        }

        private void Cancel()
        {
            OnCancel.InvokeAsync();
        }
    }
}
