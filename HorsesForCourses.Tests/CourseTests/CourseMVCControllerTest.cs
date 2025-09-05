using HorsesForCourses.Core;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Queries;
using HorsesForCourses.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using HorsesForCourses.MVC.Controllers;

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
    }
}