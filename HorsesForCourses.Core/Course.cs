namespace HorsesForCourses.Core
{
    public class Course
    {
        public Course(string title, TimeDay period)
        {
            Title = title;
            Period = period;
            _requiredSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _schedule = new List<TimeSlot>();
        }

        protected Course()
        {
            _requiredSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _schedule = new List<TimeSlot>();
        }

        public int Id { get; set; }
        public string Title { get; private set; } = null!;
        public TimeDay Period { get; private set; } = null!;
        private readonly HashSet<string> _requiredSkills;
        public IReadOnlyCollection<string> RequiredSkills => _requiredSkills;
        private readonly List<TimeSlot> _schedule;
        public IReadOnlyCollection<TimeSlot> Schedule => _schedule.AsReadOnly();

        public bool IsConfirmed { get; private set; } = false;

        public Coach? AssignedCoach { get; private set; } = null;


        public void Confirm()
        {
            if (!_schedule.Any())
                throw new InvalidOperationException("Cannot confirm course without any lessons.");

            IsConfirmed = true;
        }

        private void ConfirmConfrimation()
        {
            if (IsConfirmed)
                throw new InvalidOperationException("Cannot modify course after it has been confirmed.");
        }

        public void AddRequiredSkill(string skill)
        {
            ConfirmConfrimation();
            _requiredSkills.Add(skill);
        }

        public void ClearSchedule()
        {
            _schedule.Clear();
        }

        public void RemoveRequiredSkill(string skill)
        {
            ConfirmConfrimation();
            _requiredSkills.Remove(skill);
        }

        public void UpdateRequiredSkills(IEnumerable<string> newSkills)
        {
            ConfirmConfrimation();
            _requiredSkills.Clear();

            newSkills.ToList().ForEach(skill => AddRequiredSkill(skill));
        }

        public void AddTimeSlot(TimeSlot timeSlot)
        {
            ConfirmConfrimation();

            if (!Period.IncludesDay(timeSlot.Day))
                throw new InvalidOperationException("Cannot add a time slot for a day that is not included in the course duration.");
           
            _schedule.Add(timeSlot);
        }

        public void UpdateTimeSlot(IEnumerable<TimeSlot> newTimeSlots)
        {
            ClearSchedule();

            newTimeSlots.ToList().ForEach(ts => AddTimeSlot(ts));
        }

        public void AssignCoach(Coach coach)
        {
            if (!IsConfirmed)
                throw new InvalidOperationException("Course must be confirmed before assigning a coach.");

            if (!coach.HasAllSkills(_requiredSkills))
                throw new InvalidOperationException("Coach does not have all required skills.");

            coach.AssignCourse(this);

            AssignedCoach = coach;
        }

        public void UpdateTitle(string title)
        {
            ConfirmConfrimation();
            Title = title;
        }

        public IEnumerable<ConcreteTimeSlot> GetConcreteTimeSlots()
        {
            var concreteTimeSlots = new List<ConcreteTimeSlot>();
            for (var date = Period.StartDate; date <= Period.EndDate; date = date.AddDays(1))
            {
                foreach (var slot in Schedule)
                {
                    if (date.DayOfWeek == (DayOfWeek)slot.Day)
                    {
                        concreteTimeSlots.Add(new ConcreteTimeSlot(date, slot.Start, slot.End));
                    }
                }
            }
            return concreteTimeSlots;
        }
    }
}


/**
    Свойства:
        Id: Уникальный идентификатор курса.
        Title: Название курса.
        Period: Период, в течение которого проводится курс (вероятно, содержит начальную и конечную даты).
        RequiredSkills: Набор навыков, необходимых для прохождения курса (или навыков, которыми должен обладать тренер для ведения курса).
        Schedule: Список временных слотов (TimeSlot), составляющих расписание курса.
        IsConfirmed: Флаг, указывающий, подтвержден ли курс.
        AssignedCoach: Тренер, назначенный на этот курс.
    Методы:
        Confirm(): Подтверждает курс. Имеет проверку, что курс не может быть подтвержден без уроков.
        ConfirmConfrimation(): Внутренний метод для проверки, что курс не был подтвержден, прежде чем вносить изменения. Это предотвращает изменение подтвержденных курсов.
        AddRequiredSkill(), RemoveRequiredSkill(), UpdateRequiredSkills(): Методы для управления необходимыми навыками курса.
        AddTimeSlot(), RemoveTimeSlot(), UpdateTimeSlot(): Методы для управления расписанием курса.
        AssignCoach(): Назначает тренера на курс. Включает проверки, что курс подтвержден и что тренер обладает всеми необходимыми навыками.

        Этот класс инкапсулирует бизнес-логику, связанную с курсами, и обеспечивает соблюдение доменных инвариантов.
*/
