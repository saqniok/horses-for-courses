using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace HorsesForCourses.Blazor.Pages
{
    public partial class Coaches
    {
        [Inject]
        private ICoachService? CoachService { get; set; }

        private List<CoachSummaryResponse>? coaches;
        private string? error;

        private bool showAddCoachModal = false;
        private bool showEditCoachModal = false;
        private bool showConfirmDeleteModal = false;

        private CoachDetailsDto? editingCoach;
        private int coachIdToDelete;

        private HashSet<int> expandedCoachIds = new();
        private Dictionary<int, CoachDetailsDto> coachDetailsCache = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadCoaches();
        }

        private async Task LoadCoaches()
        {
            try
            {
                coaches = await CoachService!.GetCoachesAsync();
                error = null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private void ShowAddCoachModal() => showAddCoachModal = true;
        private void HideAddCoachModal() => showAddCoachModal = false;

        private async Task ShowEditCoachModal(int id)
        {
            try
            {
                editingCoach = await CoachService!.GetCoachDetailsAsync(id);
                showEditCoachModal = true;
                error = null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }
        private void HideEditCoachModal()
        {
            showEditCoachModal = false;
            editingCoach = null;
        }

        private void ShowDeleteConfirmModal(int id)
        {
            coachIdToDelete = id;
            showConfirmDeleteModal = true;
        }
        private void HideDeleteConfirmModal() => showConfirmDeleteModal = false;

        private async Task HandleCoachAdded()
        {
            HideAddCoachModal();
            await LoadCoaches();
        }

        private async Task HandleCoachUpdated()
        {
            if (editingCoach != null && coachDetailsCache.ContainsKey(editingCoach.Id))
            {
                coachDetailsCache[editingCoach.Id] = editingCoach;
            }
            HideEditCoachModal();
            await LoadCoaches();
        }

        private async Task HandleDeleteConfirmed()
        {
            try
            {
                await CoachService!.DeleteCoachAsync(coachIdToDelete);
                if (coachDetailsCache.ContainsKey(coachIdToDelete))
                {
                    coachDetailsCache.Remove(coachIdToDelete);
                }
                if (expandedCoachIds.Contains(coachIdToDelete))
                {
                    expandedCoachIds.Remove(coachIdToDelete);
                }
                HideDeleteConfirmModal();
                await LoadCoaches();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private async Task ToggleCoachDetails(int coachId)
        {
            if (expandedCoachIds.Contains(coachId))
            {
                expandedCoachIds.Remove(coachId);
            }
            else
            {
                expandedCoachIds.Add(coachId);
                if (!coachDetailsCache.ContainsKey(coachId))
                {
                    try
                    {
                        var details = await CoachService!.GetCoachDetailsAsync(coachId);
                        coachDetailsCache[coachId] = details;
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                        expandedCoachIds.Remove(coachId);
                    }
                }
            }
        }
    }
}
