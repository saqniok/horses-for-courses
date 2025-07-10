
namespace HorsesForCourses.Core
{
    public class Lesson
    {
        public WeekDay Day { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }

        public Lesson(WeekDay day, TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime < TimeSpan.FromHours(9) || endTime > TimeSpan.FromHours(17))
                throw new ArgumentException("Lesson time must be within working hours 9:00 - 17:00");

            if (endTime <= startTime)
                throw new ArgumentException("Lesson end time must be after start time");

            if (endTime - startTime < TimeSpan.FromHours(1))
                throw new ArgumentException("Lesson duration must be at least 1 hour.");

            Day = day;
            StartTime = startTime;
            EndTime = endTime;
        }

        public TimeSpan Duration => EndTime - StartTime;

        public bool OverlapsWith(Lesson other)
        {
            if (Day != other.Day) return false;

            return StartTime < other.EndTime && EndTime > other.StartTime;
        }
    }
}