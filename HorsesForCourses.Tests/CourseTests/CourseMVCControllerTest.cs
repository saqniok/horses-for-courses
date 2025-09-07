using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Service.Queries;
using HorsesForCourses.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using HorsesForCourses.MVC.Controllers;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

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
            var request = new CourseDto(Id: 1, Title: "C Updated", StartDate: default, EndDate: default);
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };

            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
            _courseServiceMock.Setup(s => s.UpdateAsync(course)).Returns(Task.CompletedTask);

            var result = await _controller.Edit(1, request);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ReturnsCourseView_WhenModelInvalid()
        {
            var request = new CourseDto(Id: 1, Title: "", StartDate: default, EndDate: default);

            var result = await _controller.Edit(0, request);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            var request = new CourseDto(Id: 1, Title: "C", StartDate: default, EndDate: default);
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Course?)null);

            var result = await _controller.Edit(1, request);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddSkill_RedirectsToEdit_WhenCourseExists()
        {
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);

            var result = await _controller.AddSkill(1, "C#");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
        }

        [Fact]
        public async Task RemoveSkill_RedirectsToEdit_WhenCourseExists()
        {
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };
            course.AddRequiredSkill("C#");
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);

            var result = await _controller.RemoveSkill(1, "C#");

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
        }

        [Fact]
        public async Task AddTimeSlot_RedirectsToEdit_WhenCourseExists()
        {
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);

            var result = await _controller.AddTimeSlot(1, WeekDay.Monday, 9, 12);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
        }

        [Fact]
        public async Task RemoveTimeSlot_RedirectsToEdit_WhenCourseExists()
        {
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };
            var timeSlot = new TimeSlot(WeekDay.Monday, 9, 12);
            course.AddTimeSlot(timeSlot);
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);

            var result = await _controller.RemoveTimeSlot(1, WeekDay.Monday, 9, 12);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
        }

        [Fact]
        public async Task Confirm_RedirectsToEdit_WhenCourseExistsAndHasSchedule()
        {
            // Arrange
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };
            course.AddTimeSlot(new TimeSlot(WeekDay.Monday, 9, 10)); // Add a time slot
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
            _courseServiceMock.Setup(s => s.UpdateAsync(course)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Confirm(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
            _courseServiceMock.Verify(s => s.UpdateAsync(course), Times.Once);
        }

        [Fact]
        public async Task Confirm_SetsTempDataError_WhenCourseHasNoSchedule()
        {
            // Arrange
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;


            // Act
            var result = await _controller.Confirm(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
            Assert.True(tempData.ContainsKey("Error"));
            Assert.Equal("Cannot confirm course without any lessons.", tempData["Error"]);
            _courseServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task AssignCoach_Get_ReturnsView_WhenCourseExists()
        {
            var courseDto = new CourseDto(1, "C", default, default, null, new List<TimeSlotDto>(), false, null);
            _courseServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync(courseDto);
            _coachServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<CoachSummaryResponse>());

            var result = await _controller.AssignCoach(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<CourseDto>(viewResult.Model);
        }

        [Fact]
        public async Task AssignCoach_Post_RedirectsToEdit_WhenSuccessful()
        {
            // Arrange
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };
            course.AddTimeSlot(new TimeSlot(WeekDay.Monday, 9, 10)); // Add schedule to allow confirmation
            course.Confirm(); // Confirm the course

            var coach = new Coach("John", "john@example.com") { Id = 1 };

            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
            _coachServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(coach);
            _courseServiceMock.Setup(s => s.UpdateAsync(course)).Returns(Task.CompletedTask);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;

            // Act
            var result = await _controller.AssignCoachPost(1, 1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Edit", redirectResult.ActionName);
            _courseServiceMock.Verify(s => s.UpdateAsync(course), Times.Once);
        }

        [Fact]
        public async Task AssignCoach_Post_SetsTempDataError_WhenCoachLacksSkills()
        {
            // Arrange
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };
            course.AddTimeSlot(new TimeSlot(WeekDay.Monday, 9, 10));
            course.AddRequiredSkill("C#");
            course.Confirm();

            var coach = new Coach("John", "john@example.com") { Id = 1 }; // Coach doesn't have C# skill

            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);
            _coachServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(coach);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;

            // Act
            var result = await _controller.AssignCoachPost(1, 1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AssignCoach", redirectResult.ActionName);
            Assert.True(tempData.ContainsKey("Error"));
            Assert.Equal("Coach does not have all required skills.", tempData["Error"]);
            _courseServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Delete_Get_ReturnsView_WhenCourseExists()
        {
            var course = new Course("C", new TimeDay(default, default)) { Id = 1 };
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(course);

            var result = await _controller.Delete(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Course>(viewResult.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_RedirectsToIndex()
        {
            _courseServiceMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteConfirmed(1);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task AddSkill_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Course?)null);

            var result = await _controller.AddSkill(1, "C#");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveSkill_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Course?)null);

            var result = await _controller.RemoveSkill(1, "C#");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddTimeSlot_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Course?)null);

            var result = await _controller.AddTimeSlot(1, WeekDay.Monday, 9, 12);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveTimeSlot_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Course?)null);

            var result = await _controller.RemoveTimeSlot(1, WeekDay.Monday, 9, 12);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Confirm_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Course?)null);

            var result = await _controller.Confirm(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AssignCoach_Get_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _courseServiceMock.Setup(s => s.GetDtoByIdAsync(1)).ReturnsAsync((CourseDto?)null);

            var result = await _controller.AssignCoach(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AssignCoach_Post_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Course?)null);

            var result = await _controller.AssignCoachPost(1, 1);

            Assert.IsType<NotFoundResult>(result);
        }

                [Fact]
        public async Task Delete_Get_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            _courseServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Course?)null);

            var result = await _controller.Delete(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}