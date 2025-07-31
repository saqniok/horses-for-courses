namespace HorsesForCourses.Core;

public class CourseScheduler
{
    public (bool success, string errorMessage) UpdateSchedule(Course course, IEnumerable<TimeSlot> newTimeSlotDtos)
    {
        if (newTimeSlotDtos.Any(tsDto => !Enum.IsDefined(typeof(WeekDay), tsDto.Day)))
            return (false, "One or more day values are invalid.");

        var timeSlots = new List<TimeSlot>();

        foreach (var tsDto in newTimeSlotDtos)
        {
            try
            {
                var timeSlot = new TimeSlot(tsDto.Day, tsDto.Start, tsDto.End);
                timeSlots.Add(timeSlot);
            }
            catch (ArgumentException ex)
            {
                return (false, ex.Message);
            }
        }

        course.UpdateTimeSlot(timeSlots);
        return (true, string.Empty);
    }
}
