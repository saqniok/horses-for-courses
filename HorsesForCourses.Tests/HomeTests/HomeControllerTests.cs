using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HorsesForCourses.MVC.Controllers;
using HorsesForCourses.MVC.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace HorsesForCourses.Tests.HomeTests
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object);

            // Setup HttpContext for the controller, especially for TraceIdentifier
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewResult_WithErrorViewModel()
        {
            // Arrange
            var activity = new Activity("test").Start();
            _controller.HttpContext.TraceIdentifier = "testTraceIdentifier";

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model.RequestId);
            Assert.True(model.ShowRequestId);

            activity.Stop();
        }

        [Fact]
        public void Error_ReturnsViewResult_WithNoCurrentActivity_UsesTraceIdentifier()
        {
            // Arrange
            Activity.Current = null; // Ensure no current activity
            _controller.HttpContext.TraceIdentifier = "testTraceIdentifier";

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("testTraceIdentifier", model.RequestId);
            Assert.True(model.ShowRequestId);
        }

    }
}
