using HorsesForCourses.Blazor.Dtos;
using HorsesForCourses.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq; // Added for .Any()
using System.Threading.Tasks;

namespace HorsesForCourses.Blazor.Pages
{
    /// <summary>
    /// Код-файл для компонента Blazor Coaches. Обрабатывает логику отображения, добавления, редактирования и удаления тренеров.
    /// </summary>
    public partial class Coaches
    {
        /// <summary>
        /// Внедренный сервис для взаимодействия с API-конечными точками, связанными с тренерами.
        /// </summary>
        [Inject]
        private ICoachService? CoachService { get; set; }

        /// <summary>
        /// Внедренная среда выполнения JavaScript для вызовов взаимодействия (например, отображения диалоговых окон подтверждения).
        /// </summary>
        [Inject]
        private IJSRuntime? JSRuntime { get; set; }

        private List<CoachSummaryResponse>? coaches; // Список тренеров для отображения
        private string? error; // Хранит сообщения об ошибках для отображения

        private bool showAddCoachModal = false; // Управляет видимостью модального окна добавления тренера
        private CreateCoachRequest newCoach = new() { Name = string.Empty, Email = string.Empty }; // Модель для формы добавления тренера

        private bool showEditCoachModal = false; // Управляет видимостью модального окна редактирования тренера
        private CoachDetailsDto? editingCoach; // Модель для формы редактирования тренера
        private string newSkill = string.Empty; // Используется для добавления новых навыков тренеру

        // Новые поля для модального окна подтверждения удаления
        private bool showConfirmDeleteModal = false;
        private int coachIdToDelete;

        // Поля для расширяемых деталей тренера
        private HashSet<int> expandedCoachIds = new HashSet<int>();
        private Dictionary<int, CoachDetailsDto> coachDetailsCache = new Dictionary<int, CoachDetailsDto>();

        /// <summary>
        /// Метод жизненного цикла, вызываемый при инициализации компонента. Загружает список тренеров.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            await LoadCoaches();
        }

        /// <summary>
        /// Загружает список тренеров из API.
        /// </summary>
        private async Task LoadCoaches()
        {
            try
            {
                // Используйте оператор подавления null, так как ожидается, что CoachService будет внедрен
                coaches = await CoachService!.GetCoachesAsync();
                error = null;
                expandedCoachIds.Clear(); // Очистить развернутые элементы при перезагрузке
                coachDetailsCache.Clear(); // Очистить кэш деталей при перезагрузке
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        /// <summary>
        /// Показывает модальное окно добавления тренера и инициализирует новый объект тренера.
        /// </summary>
        private void ShowAddCoachModal()
        {
            newCoach = new() { Name = string.Empty, Email = string.Empty };
            showAddCoachModal = true;
        }

        /// <summary>
        /// Скрывает модальное окно добавления тренера.
        /// </summary>
        private void HideAddCoachModal()
        {
            showAddCoachModal = false;
        }

        /// <summary>
        /// Обрабатывает отправку формы добавления тренера. Добавляет нового тренера через API и перезагружает список.
        /// </summary>
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

        /// <summary>
        /// Показывает модальное окно редактирования тренера и загружает данные выбранного тренера.
        /// </summary>
        /// <param name="id">Идентификатор тренера для редактирования.</param>
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

        /// <summary>
        /// Скрывает модальное окно редактирования тренера и очищает объект редактируемого тренера.
        /// </summary>
        private void HideEditCoachModal()
        {
            showEditCoachModal = false;
            editingCoach = null;
        }

        /// <summary>
        /// Обрабатывает отправку формы редактирования тренера. Обновляет данные тренера и навыки через API и перезагружает список.
        /// </summary>
        private async Task UpdateCoach()
        {
            if (editingCoach == null) return; // Не должно произойти, если модальное окно отображается правильно

            try
            {
                // Используйте оператор подавления null, так как ожидается, что CoachService будет внедрен
                await CoachService!.UpdateCoachAsync(editingCoach.Id, editingCoach);
                await CoachService!.UpdateCoachSkillsAsync(editingCoach.Id, new UpdateCoachSkillsDto { Skills = editingCoach.Skills });
                HideEditCoachModal();
                // Обновить кэш деталей для измененного тренера
                if (coachDetailsCache.ContainsKey(editingCoach.Id))
                {
                    coachDetailsCache[editingCoach.Id] = editingCoach; // Обновить кэшированные детали
                }
                await LoadCoaches();
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }

        /// <summary>
        /// Добавляет новый навык в список навыков тренера (только на стороне клиента).
        /// Навык сохраняется в базе данных при вызове UpdateCoach.
        /// </summary>
        /// <param name="coach">Объект тренера, к которому нужно добавить навык.</param>
        private void AddSkill(CoachDetailsDto coach)
        {
            if (!string.IsNullOrWhiteSpace(newSkill) && !coach.Skills.Contains(newSkill))
            {
                coach.Skills.Add(newSkill);
                newSkill = string.Empty;
            }
        }

        /// <summary>
        /// Обрабатывает добавление навыка при нажатии клавиши Enter в поле ввода навыка.
        /// </summary>
        /// <param name="e">Аргументы события клавиатуры.</param>
        /// <param name="coach">Объект тренера, к которому нужно добавить навык.</param>
        private void AddSkillOnEnter(KeyboardEventArgs e, CoachDetailsDto coach)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                AddSkill(coach);
            }
        }

        /// <summary>
        /// Показывает модальное окно подтверждения удаления тренера.
        /// </summary>
        /// <param name="id">Идентификатор тренера для удаления.</param>
        private void DeleteCoach(int id)
        {
            coachIdToDelete = id;
            showConfirmDeleteModal = true;
        }

        /// <summary>
        /// Выполняет удаление тренера после подтверждения.
        /// </summary>
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

        /// <summary>
        /// Отменяет удаление тренера и скрывает модальное окно подтверждения.
        /// </summary>
        private void CancelDelete()
        {
            showConfirmDeleteModal = false;
        }

        /// <summary>
        /// Переключает отображение подробной информации о тренере.
        /// </summary>
        /// <param name="coachId">Идентификатор тренера.</param>
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

        /// <summary>
        /// Удаляет навык из списка навыков тренера (только на стороне клиента).
        /// Изменение сохраняется в базе данных при вызове UpdateCoach.
        /// </summary>
        /// <param name="coach">Объект тренера, из которого нужно удалить навык.</param>
        /// <param name="skillToRemove">Строка навыка для удаления.</param>
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