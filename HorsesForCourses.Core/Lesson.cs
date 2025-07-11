
// namespace HorsesForCourses.Core
// {
//     public class Lesson
//     {
//         public WeekDay Day { get; private set; }
//         public int StartTime { get; private set; }
//         public int EndTime { get; private set; }

//         public Lesson(TimeSlot timeslot)
//         {
//             if (timeslot.Start < 9 || timeslot.End > 17)
//                 throw new ArgumentException("Lesson time must be within working hours 9:00 - 17:00");

//             if (timeslot.End <= timeslot.Start)
//                 throw new ArgumentException("Lesson end time must be after start time");

//             if (timeslot.End - timeslot.Start < 1)
//                 throw new ArgumentException("Lesson duration must be at least 1 hour.");

//             Day = day;
//             StartTime = startTime;
//             EndTime = endTime;
//         }

//         public int Duration => EndTime - StartTime;

//         public bool OverlapsWith(Lesson other)
//         {
//             if (Day != other.Day) return false;

//             return StartTime < other.EndTime && EndTime > other.StartTime;
//         }
//     }
// }