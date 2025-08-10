using HorsesForCourses.Core;

public static class TimeSlotMapper
{
    public static TimeSlot ToDomain(this TimeSlotDto dto)
    {
        return new TimeSlot(dto.Day, dto.Start, dto.End);
    }

    public static TimeSlotDto ToDto(this TimeSlot timeSlot)
    {
        return new TimeSlotDto {
            Day = timeSlot.Day,
            Start = timeSlot.Start,
            End = timeSlot.End
        };
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
