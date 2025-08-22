namespace HorsesForCourses.Blazor.Dtos
{
    public class CreateCourseRequestDto
    {
        public required string Title { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}