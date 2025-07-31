using HorsesForCourses.Core;

namespace HorsesForCourses.Application.Mappers;

public static class TimeSlotMapper
{
    public static TimeSlot ToDomain(this TimeSlotDto dto)
    {
        if (!Enum.IsDefined(typeof(WeekDay), dto.Day))
            throw new ArgumentException($"Invalid day value: {dto.Day}");

        return new TimeSlot(dto.Day, dto.Start, dto.End);
    }

    public static TimeSlotDto ToDto(this TimeSlot timeSlot)
    {
        return new TimeSlotDto(timeSlot.Day, timeSlot.Start, timeSlot.End);
    }

    public static IEnumerable<TimeSlot> ToDomain(this IEnumerable<TimeSlotDto> dtos)
    {
        return dtos.Select(dto => dto.ToDomain());
    }

    public static IEnumerable<TimeSlotDto> ToDto(this IEnumerable<TimeSlot> slots)
    {
        return slots.Select(ts => ts.ToDto());
    }
}
