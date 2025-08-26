using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace HorsesForCourses.Blazor.Pages
{
    /// Код-файл для компонента Blazor Coaches. Обрабатывает логику отображения, добавления, редактирования и удаления тренеров.
    public partial class Coaches
    {
        /// Внедренный сервис для взаимодействия с API-конечными точками, связанными с тренерами.
        [Inject]
        private ICoachService? CoachService { get; set; }

        /// Внедренная среда выполнения JavaScript для вызовов взаимодействия (например, отображения диалоговых окон подтверждения).
        [Inject]
        private IJSRuntime? JSRuntime { get; set; }

        private List<CoachSummaryResponse>? coaches;    // Список тренеров для отображения

        private string? error;                          // Хранит сообщения об ошибках для отображения

        private bool showAddCoachModal = false;         // Управляет видимостью модального окна добавления тренера
        private CreateCoachRequest newCoach = new() { Name = string.Empty, Email = string.Empty }; // Модель для формы добавления тренера

        private bool showEditCoachModal = false;        // Управляет видимостью модального окна редактирования тренера
        private CoachDetailsDto? editingCoach;          // Модель для формы редактирования тренера
        
        private string newSkill = string.Empty;         // Используется для добавления новых навыков тренеру

        // Новые поля для модального окна подтверждения удаления
        private bool showConfirmDeleteModal = false;
        private int coachIdToDelete;

        // Поля для расширяемых деталей тренера
        private HashSet<int> expandedCoachIds = new HashSet<int>();
        private Dictionary<int, CoachDetailsDto> coachDetailsCache = new Dictionary<int, CoachDetailsDto>();

        /// Метод жизненного цикла, вызываемый при инициализации компонента. Загружает список тренеров.
        protected override async Task OnInitializedAsync()
        {
            await LoadCoaches();
        }

        /// Загружает список тренеров из API.
        private async Task LoadCoaches()
        {
            try
            {
                // Используйте оператор подавления null, так как ожидается, что CoachService будет внедрен
                coaches = await CoachService!.GetCoachesAsync();
                error = null;
                expandedCoachIds.Clear();           // Очистить развернутые элементы при перезагрузке
                coachDetailsCache.Clear();          // Очистить кэш деталей при перезагрузке
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        /// Показывает модальное окно добавления тренера и инициализирует новый объект тренера.
        private void ShowAddCoachModal()
        {
            newCoach = new() { Name = string.Empty, Email = string.Empty };
            showAddCoachModal = true;
        }

        /// Скрывает модальное окно добавления тренера.
        private void HideAddCoachModal()
        {
            showAddCoachModal = false;
        }

        /// Обрабатывает отправку формы добавления тренера. Добавляет нового тренера через API и перезагружает список.
        private async Task AddCoach()
        {
            try
            {
                // Используйте оператор подавления null, так как ожидается, что CoachService будет внедрен
                await CoachService!.AddCoachAsync(newCoach);
                HideAddCoachModal();
                await LoadCoaches();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        /// Показывает модальное окно редактирования тренера и загружает данные выбранного тренера.
        private async Task ShowEditCoachModal(int id)
        {
            try
            {
                // Используйте оператор подавления null, так как ожидается, что CoachService будет внедрен
                editingCoach = await CoachService!.GetCoachDetailsAsync(id);
                showEditCoachModal = true;
                error = null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        /// Скрывает модальное окно редактирования тренера и очищает объект редактируемого тренера.
        private void HideEditCoachModal()
        {
            showEditCoachModal = false;
            editingCoach = null;
        }

        /// Обрабатывает отправку формы редактирования тренера. Обновляет данные тренера и навыки через API и перезагружает список.
        private async Task UpdateCoach()
        {
            if (editingCoach == null) return; // Не должно произойти, если модальное окно отображается правильно

            try
            {
                // Используйте оператор подавления null, так как ожидается, что CoachService будет внедрен
                await CoachService!.UpdateCoachAsync(editingCoach.Id, editingCoach);
                await CoachService!.UpdateCoachSkillsAsync(editingCoach.Id, new UpdateCoachSkillsDto { Skills = editingCoach.Skills });

                // Обновить кэш деталей для измененного тренера
                if (coachDetailsCache.ContainsKey(editingCoach.Id))
                {
                    coachDetailsCache[editingCoach.Id] = editingCoach; // Обновить кэшированные детали
                }
                
                HideEditCoachModal(); // Move this line here
                await LoadCoaches();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        /// Добавляет новый навык в список навыков тренера (только на стороне клиента).
        /// Навык сохраняется в базе данных при вызове UpdateCoach.
        private void AddSkill(CoachDetailsDto coach)
        {
            if (!string.IsNullOrWhiteSpace(newSkill) && !coach.Skills.Contains(newSkill))
            {
                coach.Skills.Add(newSkill);
                newSkill = string.Empty;
            }
        }

        /// Обрабатывает добавление навыка при нажатии клавиши Enter в поле ввода навыка.
        private void AddSkillOnEnter(KeyboardEventArgs e, CoachDetailsDto coach)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                AddSkill(coach);
            }
        }

        /// Показывает модальное окно подтверждения удаления тренера.
        private void DeleteCoach(int id)
        {
            coachIdToDelete = id;
            showConfirmDeleteModal = true;
        }

        /// Выполняет удаление тренера после подтверждения.
        private async Task ConfirmDelete()
        {
            showConfirmDeleteModal = false; // Скрыть модальное окно
            try
            {
                // Используйте оператор подавления null, так как ожидается, что CoachService будет внедрен
                await CoachService!.DeleteCoachAsync(coachIdToDelete);
                // Удалить из кэша деталей, если он там есть
                if (coachDetailsCache.ContainsKey(coachIdToDelete))
                {
                    coachDetailsCache.Remove(coachIdToDelete);
                }
                // Удалить из expandedCoachIds, если он там есть
                if (expandedCoachIds.Contains(coachIdToDelete))
                {
                    expandedCoachIds.Remove(coachIdToDelete);
                }
                await LoadCoaches();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        /// Отменяет удаление тренера и скрывает модальное окно подтверждения.
        private void CancelDelete()
        {
            showConfirmDeleteModal = false;
        }

        /// Переключает отображение подробной информации о тренере.
        private async Task ToggleCoachDetails(int coachId)
        {
            if (expandedCoachIds.Contains(coachId))
            {
                expandedCoachIds.Remove(coachId);
            }
            else
            {
                expandedCoachIds.Add(coachId);
                // Загрузить детали тренера, если они еще не кэшированы
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
                        // Если не удалось загрузить детали, удалить из expandedCoachIds, чтобы не показывать пустую секцию
                        expandedCoachIds.Remove(coachId);
                    }
                }
            }
        }


        /// Удаляет навык из списка навыков тренера (только на стороне клиента).
        /// Изменение сохраняется в базе данных при вызове UpdateCoach.
        private async Task RemoveSkill(CoachDetailsDto coach, string skillToRemove)
        {
            try
            {
                // Используйте оператор подавления null, так как ожидается, что CoachService будет внедрен
                await CoachService!.RemoveCoachSkillAsync(coach.Id, skillToRemove);
                coach.Skills.Remove(skillToRemove);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }
    }
}