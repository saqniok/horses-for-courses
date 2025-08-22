
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
    public async Task UpdateTimeSlots_ReplacesOldSlotsWithNew_OnExistingCourse()
    {
        var course = new Course("Test", new TimeDay(new DateOnly(2025, 3, 17), new DateOnly(2025, 3, 21))) { Id = 1 };
        course.AddTimeSlot(new TimeSlot(WeekDay.Tuesday, 14, 15));

        _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
        _courseServiceMock.Setup(s => s.UpdateAsync(course)).Returns(Task.CompletedTask);

        var dtos = new List<TimeSlotDto>
        {
            new TimeSlotDto { Day = WeekDay.Monday, Start = 9, End = 10 },
            new TimeSlotDto { Day = WeekDay.Friday, Start = 16, End = 17 }
        };

        var result = await _controller.UpdateTimeSlots(1, dtos);

        Assert.IsType<NoContentResult>(result);
        _courseServiceMock.Verify(s => s.UpdateAsync(course), Times.Once);

        Assert.Equal(2, course.Schedule.Count);
        Assert.Contains(course.Schedule, ts => ts.Day == WeekDay.Monday && ts.Start == 9 && ts.End == 10);
        Assert.Contains(course.Schedule, ts => ts.Day == WeekDay.Friday && ts.Start == 16 && ts.End == 17);
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
        var course = new Course("Test", new TimeDay(default, default)) { Id = 1 };
        var coach = new Coach("CoachName", "coach@example.com") { Id = 2 };
        course.AddTimeSlot(new TimeSlot(WeekDay.Monday, 9, 10));
        _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
        _coachServiceMock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(coach);
        _courseServiceMock.Setup(s => s.UpdateAsync(course)).Returns(Task.CompletedTask);

        var dto = new AssignCoachRequest(2);
        course.Confirm();

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



    [Fact]
    public async Task GetById_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Course?)null);

        var result = await _controller.GetById(1);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Add_ReturnsCreatedAtAction()
    {
        var dto = new CreateCourseRequest(
            Title: "New Course",
            startDate: DateOnly.FromDateTime(DateTime.Today),
            endDate: DateOnly.FromDateTime(DateTime.Today.AddDays(1)));

        _courseServiceMock.Setup(s => s.CreateAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);

        var result = await _controller.Add(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnDto = Assert.IsType<CourseDto>(createdResult.Value);
        Assert.Equal("New Course", returnDto.Title);
    }

    [Fact]
    public async Task UpdateSkills_ReturnsNoContent_WhenCourseExists()
    {
        var course = new Course("Test", new TimeDay(default, default)) { Id = 1 };
        _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
        _courseServiceMock.Setup(s => s.UpdateAsync(course)).Returns(Task.CompletedTask);

        var skills = new List<string> { "Skill1", "Skill2" };

        var result = await _controller.UpdateSkills(1, skills);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(skills, course.RequiredSkills);
    }

    [Fact]
    public async Task UpdateSkills_ReturnsBadRequest_WhenInvalidOperation()
    {
        var course = new Course("Test", new TimeDay(default, default)) { Id = 1 };
        _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
        _courseServiceMock.Setup(s => s.UpdateAsync(course)).ThrowsAsync(new InvalidOperationException("Invalid operation"));

        var skills = new List<string> { "Skill1" };

        var result = await _controller.UpdateSkills(1, skills);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid operation", badRequest.Value);
    }

    [Fact]
    public async Task GetPaged_ReturnsOk()
    {
        var courses = new List<Course>
    {
        new Course("C1", new TimeDay(default, default)) { Id = 1 },
        new Course("C2", new TimeDay(default, default)) { Id = 2 }
    };

        var pagedResult = new PagedResult<Course>(courses, 2, 1, courses.Count);
        _courseServiceMock.Setup(s => s.GetPagedAsync(It.IsAny<PageRequest>(), default)).ReturnsAsync(pagedResult);

        var result = await _controller.GetPaged();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnPaged = Assert.IsType<PagedResult<Course>>(okResult.Value);
        Assert.Equal(2, returnPaged.Items.Count());
    }
}
