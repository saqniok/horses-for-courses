
namespace HorsesForCourses.Core
{
    public class Course
    {
        // Конструктор для приложения
        public Course(string title, Period period)
        {
            Title = title;
            Period = period;
            _requiredSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _schedule = new List<TimeSlot>();
        }

        // Для EF Core
        protected Course()
        {
            _requiredSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _schedule = new List<TimeSlot>();
        }

        public int Id { get; set; }

        // Сделал private set, чтобы EF мог присвоить значение при materialization
        public string Title { get; private set; } = null!;

        // Если Period — value object, лучше настроить Owned mapping в OnModelCreating.
        public Period Period { get; private set; } = null!;

        // Поле, которое ты маппишь через Converters.HashSetToString()
        private readonly HashSet<string> _requiredSkills;
        public IReadOnlyCollection<string> RequiredSkills => _requiredSkills;

        // Список уроков; ты игнорируешь Schedule в OnModelCreating, но поле должно быть валидным.
        private readonly List<TimeSlot> _schedule;
        public IReadOnlyCollection<TimeSlot> Schedule => _schedule.AsReadOnly();

        public bool IsConfirmed { get; private set; } = false;

        // Навигация к тренеру — private set подходит EF
        public Coach? AssignedCoach { get; private set; } = null;


        public void Confirm()
        {
            if (!_schedule.Any()) throw new InvalidOperationException("Cannot confirm course without any lessons.");

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

            foreach (var skill in newSkills)
                AddRequiredSkill(skill);
        }

        public void AddTimeSlot(TimeSlot timeSlot)
        {
            ConfirmConfrimation();
            _schedule.Add(timeSlot);
        }

        public void RemoveTimeSlot(TimeSlot timeSlot)
        {
            ConfirmConfrimation();
            _schedule.Remove(timeSlot);
        }

        public void UpdateTimeSlot(IEnumerable<TimeSlot> newTimeSlots)
        {
            ClearSchedule();

            foreach (var timeSlot in newTimeSlots)
                AddTimeSlot(timeSlot);
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
    }
}
