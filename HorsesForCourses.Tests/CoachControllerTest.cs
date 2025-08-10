
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class CoachControllerTests
{
    private readonly Mock<ICoachService> _serviceMock;
    private readonly CoachController _controller;

    public CoachControllerTests()
    {
        _serviceMock = new Mock<ICoachService>();
        _controller = new CoachController(_serviceMock.Object);
    }

    [Fact]
    public async Task Add_ShouldReturnCreatedAtAction()
    {
        var dto = new CreateCoachDto { Name = "John", Email = "john@example.com" };
        var coach = CoachMapper.ToDomain(dto);

        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<Coach>()))
                    .Returns(Task.CompletedTask)
                    .Callback<Coach>(c => c.Id = 1);


        var result = await _controller.Add(dto);


        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CoachController.GetById), createdResult.ActionName);
        var returnedDto = Assert.IsType<CoachSummaryDto>(createdResult.Value);
        Assert.Equal(1, returnedDto.Id);
        Assert.Equal("John", returnedDto.Name);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithCoaches()
    {

        var coaches = new List<Coach>
        {
            new Coach("John", "john@example.com"),
            new Coach("Jane", "jane@example.com")
        };
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(coaches);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsAssignableFrom<IEnumerable<CoachSummaryDto>>(okResult.Value);
        Assert.Equal(2, list.Count());
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenCoachExists()
    {

        var coach = new Coach("John", "john@example.com");
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(coach);


        var result = await _controller.GetById(1);


        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<CoachDetailsDto>(okResult.Value);
        Assert.Equal("John", dto.Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenCoachDoesNotExist()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Coach?)null);

        var result = await _controller.GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateCoachSkills_ShouldReturnNoContent_WhenCoachExists()
    {
        var coach = new Coach("John", "john@example.com");
        _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(coach);

        var dto = new UpdateCoachSkillsDto { Skills = new List<string> { "Strategy" } };

        _serviceMock.Setup(s => s.UpdateAsync(coach)).Returns(Task.CompletedTask);

        var result = await _controller.UpdateCoachSkills(1, dto);

        Assert.IsType<NoContentResult>(result);
        _serviceMock.Verify(s => s.UpdateAsync(coach), Times.Once);
        Assert.Contains("Strategy", coach.Skills);
    }

    [Fact]
    public async Task UpdateCoachSkills_ShouldReturnNotFound_WhenCoachDoesNotExist()
    {
        _serviceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((Coach?)null);
        var dto = new UpdateCoachSkillsDto { Skills = new List<string> { "Bluffing" } };

        var result = await _controller.UpdateCoachSkills(5, dto);

        Assert.IsType<NotFoundResult>(result);
        _serviceMock.Verify(s => s.UpdateAsync(It.IsAny<Coach>()), Times.Never);
    }
}
