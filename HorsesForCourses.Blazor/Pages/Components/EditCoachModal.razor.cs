using HorsesForCourses.Blazor.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace HorsesForCourses.Blazor.Pages.Components
{
    public partial class EditCoachModal
    {
        [Parameter]
        public bool Show { get; set; }

        [Parameter, EditorRequired]
        public CoachDetailsDto EditingCoach { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback<CoachDetailsDto> OnConfirm { get; set; }

        [Parameter]
        public EventCallback<string> OnAddSkill { get; set; }

        [Parameter]
        public EventCallback<string> OnRemoveSkill { get; set; }

        private string newSkill = string.Empty;

        private async Task HandleConfirm()
        {
            await OnConfirm.InvokeAsync(EditingCoach);
        }

        private async Task AddSkill()
        {
            if (!string.IsNullOrWhiteSpace(newSkill))
            {
                await OnAddSkill.InvokeAsync(newSkill);
                newSkill = string.Empty;
            }
        }

        private async Task AddSkillOnEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await AddSkill();
            }
        }

        private async Task RemoveSkill(string skill)
        {
            await OnRemoveSkill.InvokeAsync(skill);
        }
    }
}