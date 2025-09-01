using HorsesForCourses.Core;

namespace HorsesForCourses.Service.DTOs;

public class TimeSlotDto
{
    public WeekDay Day { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
}