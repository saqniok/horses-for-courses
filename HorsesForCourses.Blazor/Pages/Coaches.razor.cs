using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace HorsesForCourses.Blazor.Pages
{
    //'Partial class' means that this class is part of the determination of the component,
    //which is also in the corresponding .razor file.
    public partial class Coaches
    {

        //[inject] -this is an attribute indicated by Blazor to introduce (inject) service,
        //registered in the container container. In this case, this is a service for working with coaches.
        [Inject]
        private ICoachService? CoachService { get; set; }

        private List<CoachSummaryResponse>? coaches;
        private string? error;

        //Flags for controlling the visibility of modal windows.
        private bool showAddCoachModal = false;
        private bool showEditCoachModal = false;
        private bool showConfirmDeleteModal = false;


        //Stores the data of the coach, which is currently edited.
        private CoachDetailsDto? editingCoach;

        //Store the ID of the coach, whom they are going to remove.
        private int coachIdToDelete;


        // --- Collections for managing interactivity UI ---

        //Hashset for storing ID coaches, whose details are currently disclosed (shown).
        //Hashset is used to quickly add, delete and check the presence of an element.
        private HashSet<int> expandedCoachIds = new();

        //`Dictionary` for caching the details of the coaches that have already been loaded.
        //Key -ID coach, value -DTO with details. This allows you to not upload data again.
        private Dictionary<int, CoachDetailsDto> coachDetailsCache = new();


        ////////////// CHECK FOR KEEPING THIS CODE //////////////
        //OnInitializedAsync is one of the methods of the Blazor component life cycle.
        //It is called once after the component was initialized.
        //The perfect place for asynchronous data load.
        protected override async Task OnInitializedAsync()
        {
            // Вызываем метод для загрузки списка тренеров.
            await LoadCoaches();
        }
        ////////////// CHECK FOR KEEPING THIS CODE //////////////

        private async Task LoadCoaches()
        {
            try
            {
                //Call the `service` method for obtaining data.
                //`CoachService!` -the operator `!` Tells the compiler that we are sure that CoachService will not be null.
                coaches = await CoachService!.GetCoachesAsync();
                //If the load was successful, we clean the error message.
                error = null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }


        // --- Events of events to add a coach ---

        // This method is called after successfully adding a new coach in a subsidiary (form).
        //`async viod` should be used with caution, but here it is acceptable for events.
        private async void HandleCoachAdded()
        {
            showAddCoachModal = false;

            //reboot the list of trainers to display the new one.
            await LoadCoaches();

            //notifies Blazor that the condition of the component has changed and it needs to be redrawn.
            StateHasChanged();
        }


        private void ShowAddCoachModal()
        {
            showAddCoachModal = true;
        }

        private void HideAddCoachModal()
        {
            showAddCoachModal = false;
        }

        // --- Logic to display the details of the coach ---

        // Switchs the visibility of detailed information about the coach.
        private async Task ToggleCoachDetails(int coachId)
        {
            //If the details are already open, we turn them off (delete the ID from Hashset).
            if (expandedCoachIds.Contains(coachId))
                expandedCoachIds.Remove(coachId);

            else
            {
                expandedCoachIds.Add(coachId);
                
                //Check if there are details of this coach in the cache.
                if (!coachDetailsCache.ContainsKey(coachId))
                {
                    // Если в кэше нет, загружаем их с сервера.
                    try
                    {
                        //Save the loaded parts in the cache.
                        coachDetailsCache[coachId] = await CoachService!.GetCoachDetailsAsync(coachId);
                    }
                    catch (Exception ex)
                    {
                        error = $"Error loading coach details: {ex.Message}";
                    }
                }
            }
        }

        // --- Events for editing the coach ---

        //shows a modal window for editing a coach.
        private async void ShowEditCoachModal(int coachId)
        {
            try
            {
                // Always load fresh coach data from the server to ensure we have the latest information
                editingCoach = await CoachService!.GetCoachDetailsAsync(coachId);

                // Update the cache with the fresh data
                coachDetailsCache[coachId] = editingCoach;

                showEditCoachModal = true;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                error = $"Error loading coach details for editing: {ex.Message}";
                StateHasChanged();
            }
        }

        private void HideEditCoachModal()
        {
            showEditCoachModal = false;
            editingCoach = null;
        }

        // This method is called after a successful update of the coach data.
        private async void HandleCoachUpdated()
        {
            HideEditCoachModal();

            //reboot the list to display the updated data.
            await LoadCoaches();
            StateHasChanged();
        }

        // --- Events of events for removing the coach ---

        private void ShowDeleteConfirmModal(int coachId)
        {
            //Save the ID of the coach that needs to be removed.
            coachIdToDelete = coachId;
            showConfirmDeleteModal = true;
        }

        private void HideDeleteConfirmModal()
        {
            showConfirmDeleteModal = false;
        }

        // called after confirmation of removal.
        private async void HandleDeleteConfirmed()
        {
            try
            {
                //Call the service method for removal.
                await CoachService!.DeleteCoachAsync(coachIdToDelete);
                showConfirmDeleteModal = false;

                // reboot the list so that the remote coach disappears.
                await LoadCoaches();
                StateHasChanged();
            }
            catch (Exception ex)
            {                
                error = $"Error deleting coach: {ex.Message}";
                showConfirmDeleteModal = false;
                StateHasChanged(); // check for keeping this line
            }
        }
    }
}
