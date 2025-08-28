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

        [Parameter]
        public CoachDetailsDto? EditingCoach { get; set; }

        [Parameter]
        public EventCallback OnCoachUpdated { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        private string newSkill = string.Empty;
        private string? error;

        private async Task UpdateCoach()
        {
            if (EditingCoach == null) return;

            try
            {
                await CoachService!.UpdateCoachAsync(EditingCoach.Id, EditingCoach);
                await CoachService!.UpdateCoachSkillsAsync(EditingCoach.Id, new UpdateCoachSkillsDto { Skills = EditingCoach.Skills });
                await OnCoachUpdated.InvokeAsync();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private void AddSkill()
        {
            if (EditingCoach != null && !string.IsNullOrWhiteSpace(newSkill) && !EditingCoach.Skills.Contains(newSkill))
            {
                EditingCoach.Skills.Add(newSkill);
                newSkill = string.Empty;
            }
        }

        private void AddSkillOnEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                AddSkill();
            }
        }

        private async Task RemoveSkill(string skillToRemove)
        {
            if (EditingCoach == null) return;

            try
            {
                await CoachService!.RemoveCoachSkillAsync(EditingCoach.Id, skillToRemove);
                EditingCoach.Skills.Remove(skillToRemove);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        private void Cancel()
        {
            OnCancel.InvokeAsync();
        }
    }
}
