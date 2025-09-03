using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Queries;
using HorsesForCourses.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using HorsesForCourses.MVC.Controllers;

namespace HorsesForCourses.Tests
{
    public class CoachesMVCControllerTest
    { 
        private readonly Mock<IGetCoachSummariesQuery> _getCoachSummariesMock;
        private readonly Mock<ICoachService> _coachServiceMock;
        private readonly CoachMVCController _controller;

        public CoachesMVCControllerTest()
        {
            _getCoachSummariesMock = new Mock<IGetCoachSummariesQuery>();
            _coachServiceMock = new Mock<ICoachService>();
            _controller = new CoachMVCController(_getCoachSummariesMock.Object, _coachServiceMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithPagedResult()
        {
            var pageRequest = new PageRequest(1, 25);
            var pagedResult = new PagedResult<CoachSummaryResponse>(
                new List<CoachSummaryResponse>
                {
                    new CoachSummaryResponse { Id = 1, Name = "John", Email = "john@example.com", NumberOfCoursesAssignedTo = 2 }
                },
                1, 1, 25);

            _getCoachSummariesMock.Setup(q => q.All(pageRequest)).ReturnsAsync(pagedResult);

            var result = await _controller.Index(1, 25);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PagedResult<CoachSummaryResponse>>(viewResult.Model);
            Assert.Single(model.Items);
            Assert.Equal("John", model.Items.First().Name);
        }

        [Fact]
        public async Task Details_ReturnsView_WhenCoachExists()
        {
            var coachDto = new CoachDetailsDto
            {
                Id = 1,
                Name = "John",
                Email = "john@example.com",
                Skills = new List<string> { "Skill1" },
                Courses = new List<CourseShortDto>()
            };
            _coachServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync(coachDto);

            var result = await _controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CoachDetailsDto>(viewResult.Model);
            Assert.Equal("John", model.Name);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenCoachDoesNotExist()
        {
            _coachServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync((CoachDetailsDto?)null);

            var result = await _controller.Details(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_Get_ReturnsView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ReturnsRedirect_WhenModelValid()
        {
            var request = new CreateCoachRequest { Name = "John", Email = "john@example.com" };
            _coachServiceMock.Setup(s => s.CreateAsync(It.IsAny<Coach>())).Returns(Task.CompletedTask);

            var result = await _controller.Create(request);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ReturnsView_WhenModelInvalid()
        {
            var request = new CreateCoachRequest { Name = "", Email = "john@example.com" };
            _controller.ModelState.AddModelError("Name", "Required");

            var result = await _controller.Create(request);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(request, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_ReturnsView_WhenCoachExists()
        {
            var coachDto = new CoachDetailsDto
            {
                Id = 1,
                Name = "John",
                Email = "john@example.com",
                Skills = new List<string>(),
                Courses = new List<CourseShortDto>()
            };
            _coachServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync(coachDto);

            var result = await _controller.Edit(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CoachDetailsDto>(viewResult.Model);
            Assert.Equal("John", model.Name);
        }

        [Fact]
        public async Task Edit_Get_ReturnsNotFound_WhenCoachDoesNotExist()
        {
            _coachServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync((CoachDetailsDto?)null);

            var result = await _controller.Edit(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsRedirect_WhenModelValid()
        {
            var request = new CreateCoachRequest { Name = "Jane", Email = "jane@example.com" };
            var coach = new Coach("John", "john@example.com");
            _coachServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(coach);
            _coachServiceMock.Setup(s => s.UpdateAsync(coach)).Returns(Task.CompletedTask);

            var result = await _controller.Edit(1, request);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFound_WhenIdInvalid()
        {
            var request = new CreateCoachRequest { Name = "Jane", Email = "jane@example.com" };

            var result = await _controller.Edit(0, request);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFound_WhenCoachDoesNotExist()
        {
            var request = new CreateCoachRequest { Name = "Jane", Email = "jane@example.com" };
            _coachServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Coach?)null);

            var result = await _controller.Edit(1, request);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsView_WhenModelInvalid()
        {
            var request = new CreateCoachRequest { Name = "", Email = "jane@example.com" };
            var coachDto = new CoachDetailsDto
            {
                Id = 1,
                Name = "John",
                Email = "john@example.com",
                Skills = new List<string>(),
                Courses = new List<CourseShortDto>()
            };
            _controller.ModelState.AddModelError("Name", "Required");
            _coachServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync(coachDto);

            var result = await _controller.Edit(1, request);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(coachDto, viewResult.Model);
        }

        [Fact]
        public async Task Delete_Get_ReturnsView_WhenCoachExists()
        {
            var coachDto = new CoachDetailsDto
            {
                Id = 1,
                Name = "John",
                Email = "john@example.com",
                Skills = new List<string>(),
                Courses = new List<CourseShortDto>()
            };
            _coachServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync(coachDto);

            var result = await _controller.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CoachDetailsDto>(viewResult.Model);
            Assert.Equal("John", model.Name);
        }

        [Fact]
        public async Task Delete_Get_ReturnsNotFound_WhenCoachDoesNotExist()
        {
            _coachServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync((CoachDetailsDto?)null);

            var result = await _controller.Delete(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsRedirect_WhenCoachExists()
        {
            var coach = new Coach("John", "john@example.com");
            _coachServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(coach);
            _coachServiceMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteConfirmed(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_ReturnsNotFound_WhenCoachDoesNotExist()
        {
            _coachServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Coach?)null);

            var result = await _controller.DeleteConfirmed(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddSkill_ReturnsRedirect_WhenCoachExistsAndSkillValid()
        {
            var coach = new Coach("John", "john@example.com");
            _coachServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(coach);
            _coachServiceMock.Setup(s => s.UpdateAsync(coach)).Returns(Task.CompletedTask);

            var result = await _controller.AddSkill(1, "NewSkill");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues!["id"]);
        }

        [Fact]
        public async Task AddSkill_ReturnsNotFound_WhenCoachDoesNotExist()
        {
            _coachServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Coach?)null);

            var result = await _controller.AddSkill(1, "NewSkill");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveSkill_ReturnsRedirect()
        {

            _coachServiceMock.Setup(s => s.RemoveSkillAsync(1, "Skill")).Returns(Task.CompletedTask);

            var result = await _controller.RemoveSkill(1, "Skill");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
            Assert.Equal(1, redirectResult.RouteValues!["id"]);
        }
    }
}