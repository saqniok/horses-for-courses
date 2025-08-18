
using Microsoft.AspNetCore.Mvc;
using Moq;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.DTOs;
using HorsesForCourses.WebApi.Service;

public class CourseControllerTests
{
    private readonly Mock<ICourseService> _courseServiceMock = new();
    private readonly Mock<ICoachService> _coachServiceMock = new();
    private readonly CourseController _controller;

    public CourseControllerTests()
    {
        _controller = new CourseController(_courseServiceMock.Object, _coachServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithCourses()
    {
        // Arrange
        var courses = new List<CourseDto>
        {
            new CourseDto(Id: 1, Title: "Course1", StartDate: default, EndDate: default),
            new CourseDto(Id: 2, Title: "Course2", StartDate: default, EndDate: default)
        };

        _courseServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(courses);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnCourses = Assert.IsAssignableFrom<IEnumerable<CourseDto>>(okResult.Value);
        Assert.Equal(2, returnCourses.Count());
    }

    [Fact]
    public async Task AddTimeSlot_ReturnsNoContent_WhenCourseExists()
    {
        // Arrange
        var course = new Course("Test", new TimeDay(default, default)) { Id = 1 };
        _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
        _courseServiceMock.Setup(s => s.UpdateAsync(course)).Returns(Task.CompletedTask);

        var dto = new TimeSlotDto
        {
            Day = WeekDay.Monday,
            Start = 9,
            End = 10
        };

        // Act
        var result = await _controller.AddTimeSlot(1, dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _courseServiceMock.Verify(s => s.UpdateAsync(course), Times.Once);
        Assert.Single(course.Schedule);
    }

    [Fact]
    public async Task ConfirmCourse_ReturnsNoContent_WhenCourseExists()
    {
        // Arrange
        var course = new Course("Test", new TimeDay(default, default)) { Id = 1 };
        course.AddTimeSlot(new TimeSlot(WeekDay.Monday, 9, 10));
        _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
        _courseServiceMock.Setup(s => s.UpdateAsync(course)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ConfirmCourse(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _courseServiceMock.Verify(s => s.UpdateAsync(course), Times.Once);
        Assert.True(course.IsConfirmed);
    }

    [Fact]
    public async Task AssignCoach_ReturnsNoContent_WhenCourseAndCoachExist()
    {
        // Arrange
        var course = new Course("Test", new TimeDay(default, default)) { Id = 1 };
        var coach = new Coach("CoachName", "coach@example.com") { Id = 2 };
        course.AddTimeSlot(new TimeSlot(WeekDay.Monday, 9, 10));
        _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
        _coachServiceMock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(coach);
        _courseServiceMock.Setup(s => s.UpdateAsync(course)).Returns(Task.CompletedTask);

        var dto = new AssignCoachRequest(2);
        course.Confirm(); // Confirm the course before assigning coach

        var result = await _controller.AssignCoach(1, dto);

        Assert.IsType<NoContentResult>(result);
        _courseServiceMock.Verify(s => s.UpdateAsync(course), Times.Once);
        Assert.Equal(coach, course.AssignedCoach);
    }

    [Fact]
    public async Task AssignCoach_ReturnsNotFound_WhenCoachDoesNotExist()
    {
        var course = new Course("Test", new TimeDay(default, default)) { Id = 1 };

        _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
        _coachServiceMock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync((Coach?)null);

        var dto = new AssignCoachRequest(2);

        var result = await _controller.AssignCoach(1, dto);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Coach not found.", notFoundResult.Value);
    }
}
