
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Queries;
using HorsesForCourses.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using HorsesForCourses.MVC.Controllers;
using HorsesForCourses.Core;

namespace HorsesForCourses.Tests
{
    public class CourseMVCControllerTest
    {
        private readonly Mock<IGetCourseSummariesQuery> _getCourseSummariesMock;
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly Mock<ICoachService> _coachServiceMock;
        private readonly CourseMVCController _controller;

        public CourseMVCControllerTest()
        {
            _getCourseSummariesMock = new Mock<IGetCourseSummariesQuery>();
            _coachServiceMock = new Mock<ICoachService>();
            _courseServiceMock = new Mock<ICourseService>();
            _controller = new CourseMVCController(_getCourseSummariesMock.Object, _courseServiceMock.Object, _coachServiceMock.Object);
        }

        [Fact]
        public async Task Index_ReturnCoursesViewWithPagedResult()
        {
            var pageRequest = new PageRequest(1, 5);
            var pagedResult = new PagedResult<CourseSummaryResponse>(
                new List<CourseSummaryResponse>
                {
                    new CourseSummaryResponse {
                        Id = 1,
                        Title = "C",
                        StartDate = default,
                        EndDate = default,
                        IsConfirmed = false,
                        AssignedCoachName = "John",
                        NumberOfLessons = 0 }
                },
                1, 1, 5);

            _getCourseSummariesMock.Setup(q => q.All(pageRequest)).ReturnsAsync(pagedResult);

            var result = await _controller.Index(1, 5);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PagedResult<CourseSummaryResponse>>(viewResult.Model);
            Assert.Single(model.Items);
            Assert.Equal("C", model.Items.First().Title);
        }

        [Fact]
        public async Task Details_ReturnsView_WhenCourseExists()
        {
            var courseDto = new CourseDto(
                Id: 1,
                Title: "C",
                StartDate: default,
                EndDate: default,
                IsConfirmed: false,
                Coach: new CoachShortDto(1, "John"),
                Schedule: new List<TimeSlotDto>()
            );
            _courseServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync(courseDto);

            var result = await _controller.Details(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CourseDto>(viewResult.Model);
            Assert.Equal("C", model.Title);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _courseServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync((CourseDto?)null);

            var result = await _controller.Details(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_Get_ReturnCourseView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Post_ReturnsCourseView_WhenModelValid()
        {
            var request = new CreateCourseRequest(Title: "C", startDate: default, endDate: default);
            _courseServiceMock.Setup(s => s.CreateAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);

            var result = await _controller.Create(request);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ReturnsCourseView_WhenModelInvalid()
        {
            var request = new CreateCourseRequest(Title: "", startDate: default, endDate: default);
            _controller.ModelState.AddModelError("Title", "Required");

            var result = await _controller.Create(request);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(request, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_ReturnsCourseView_WhenCourseExists()
        {
            var courseDto = new CourseDto(
                Id: 1,
                Title: "C",
                StartDate: default,
                EndDate: default,
                IsConfirmed: false,
                Coach: new CoachShortDto(1, "John"),
                Schedule: new List<TimeSlotDto>()
            );
            _courseServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync(courseDto);

            var result = await _controller.Edit(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CourseDto>(viewResult.Model);
            Assert.Equal("C", model.Title);
        }

        [Fact]
        public async Task Edit_Get_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _courseServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync((CourseDto?)null);

            var result = await _controller.Edit(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsRedirect_WhenModelValid()
        {
            var request = new CourseDto(Id: 1, Title: "C Updated", StartDate: default, EndDate: default, IsConfirmed: false, Coach: null, Schedule: new List<TimeSlotDto>());
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };

            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
            _courseServiceMock.Setup(s => s.UpdateAsync(course)).Returns(Task.CompletedTask);

            var result = await _controller.Edit(1, request);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}