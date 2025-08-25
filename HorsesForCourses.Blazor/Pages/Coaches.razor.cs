using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HorsesForCourses.Blazor.Pages
{
    public partial class Coaches
    {
        [Inject]
        private ICoachService? CoachService { get; set; }

        [Inject]
        private IJSRuntime? JSRuntime { get; set; }

        private List<CoachSummaryResponse>? coaches;
        private string? error;

        private bool showAddCoachModal = false;
        private CreateCoachRequest newCoach = new() { Name = string.Empty, Email = string.Empty };

        private bool showEditCoachModal = false;
        private CoachDetailsDto? editingCoach;
        private string newSkill = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadCoaches();
        }

        private async Task LoadCoaches()
        {
            try
            {
                coaches = await CoachService.GetCoachesAsync();
                error = null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private void ShowAddCoachModal()
        {
            newCoach = new() { Name = string.Empty, Email = string.Empty };
            showAddCoachModal = true;
        }

        private void HideAddCoachModal()
        {
            showAddCoachModal = false;
        }

        private async Task AddCoach()
        {
            try
            {
                await CoachService.AddCoachAsync(newCoach);
                HideAddCoachModal();
                await LoadCoaches();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private async Task ShowEditCoachModal(int id)
        {
            try
            {
                editingCoach = await CoachService.GetCoachDetailsAsync(id);
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

        private async Task UpdateCoach()
        {
            if (editingCoach == null) return;

            try
            {
                await CoachService.UpdateCoachAsync(editingCoach.Id, editingCoach);
                await CoachService.UpdateCoachSkillsAsync(editingCoach.Id, new UpdateCoachSkillsDto { Skills = editingCoach.Skills });
                HideEditCoachModal();
                await LoadCoaches();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private void AddSkill(CoachDetailsDto coach)
        {
            if (!string.IsNullOrWhiteSpace(newSkill) && !coach.Skills.Contains(newSkill))
            {
                coach.Skills.Add(newSkill);
                newSkill = string.Empty;
            }
        }

        private void AddSkillOnEnter(KeyboardEventArgs e, CoachDetailsDto coach)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                AddSkill(coach);
            }
        }

        private async Task DeleteCoach(int id)
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this coach?");
            if (confirmed)
            {
                try
                {
                    await CoachService.DeleteCoachAsync(id);
                    await LoadCoaches();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
        }

        private async Task RemoveSkill(CoachDetailsDto coach, string skillToRemove)
        {
            try
            {
                await CoachService.RemoveCoachSkillAsync(coach.Id, skillToRemove);
                coach.Skills.Remove(skillToRemove);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }
    }
}