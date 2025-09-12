using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using HorsesForCourses.MVC.Controllers;
using HorsesForCourses.Service.Interfaces;
using HorsesForCourses.MVC.Models.ViewModels;
using HorsesForCourses.Service.DTOs;
using HorsesForCourses.Core;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures; // Added for IHtmlHelper
using Microsoft.AspNetCore.Mvc.Routing; // Added for UrlActionContext

namespace HorsesForCourses.Tests.AccountTests
{
    public class AccountControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockUserService = new Mock<IUserService>();
            _controller = new AccountController(_mockAuthService.Object, _mockUserService.Object);

            // Setup HttpContext for the controller
            var httpContext = new DefaultHttpContext();
            
            // Mock IUrlHelper
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns((UrlActionContext urlContext) => $"/{urlContext.Controller}/{urlContext.Action}");
            _controller.Url = mockUrlHelper.Object;

            // Mock ITempDataDictionary
            var mockTempData = new Mock<ITempDataDictionary>();
            _controller.TempData = mockTempData.Object;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _controller.HttpContext.Request.Scheme = "https"; // Mock HTTPS scheme
            _controller.HttpContext.RequestServices = new Mock<IServiceProvider>().Object; // Required for ChallengeResult
        }

        [Fact]
        public void Login_GET_ReturnsViewResult()
        {
            var result = _controller.Login();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_POST_ValidModel_SuccessfulLogin_RedirectsToHomeIndex()
        {
            var model = new LoginViewModel { Email = "test@example.com", Password = "Password123" };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") }, "Cookies"));
            _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync((claimsPrincipal, new Dictionary<string, string>()));

            // Act
            var result = await _controller.Login(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            _mockAuthService.Verify(s => s.LoginAsync(It.IsAny<LoginDto>()), Times.Once);
        }

        [Fact]
        public async Task Login_POST_ValidModel_FailedLogin_ReturnsViewWithModelError()
        {
            var model = new LoginViewModel { Email = "test@example.com", Password = "WrongPassword" };
            var errors = new Dictionary<string, string> { { string.Empty, "Invalid login attempt." } };
            _mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync((null, errors));

            // Act
            var result = await _controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Contains("Invalid login attempt.", _controller.ModelState[string.Empty]!.Errors.Select(e => e.ErrorMessage));
            _mockAuthService.Verify(s => s.LoginAsync(It.IsAny<LoginDto>()), Times.Once);
        }

        [Fact]
        public void Register_GET_ReturnsViewResult()
        {
            var result = _controller.Register();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Register_POST_ValidModel_SuccessfulRegistration_RedirectsToHomeIndex()
        {
            var model = new RegisterAccountViewModel { Name = "Test User", Email = "register@example.com", Pass = "Password123", ConfirmPass = "Password123" };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") }, "Cookies"));
            _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<RegisterAccountDto>()))
                .ReturnsAsync((claimsPrincipal, new Dictionary<string, string>()));

            // Act
            var result = await _controller.Register(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            _mockAuthService.Verify(s => s.RegisterAsync(It.IsAny<RegisterAccountDto>()), Times.Once);
        }

        [Fact]
        public async Task Register_POST_ValidModel_EmailAlreadyExists_ReturnsViewWithModelError()
        {
            var model = new RegisterAccountViewModel { Name = "Test User", Email = "existing@example.com", Pass = "Password123", ConfirmPass = "Password123" };
            var errors = new Dictionary<string, string> { { "Email", "User with this email already exists." } };
            _mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<RegisterAccountDto>()))
                .ReturnsAsync((null, errors));

            // Act
            var result = await _controller.Register(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Contains("User with this email already exists.", _controller.ModelState["Email"]!.Errors.Select(e => e.ErrorMessage));
            _mockAuthService.Verify(s => s.RegisterAsync(It.IsAny<RegisterAccountDto>()), Times.Once);
        }

        [Fact]
        public async Task Logout_POST_RedirectsToHomeIndex()
        {
            _mockAuthService.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            _mockAuthService.Verify(s => s.SignOutAsync(), Times.Once);
        }

        [Fact]
        public async Task DownloadUserData_GET_UserNotAuthenticated_ReturnsUnauthorized()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // Not authenticated

            var result = await _controller.DownloadUserData();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DownloadUserData_GET_UserAuthenticated_ReturnsFileResult()
        {
            var userId = 1;
            var userEmail = "test@example.com";
            var user = new User("Test User", userEmail, "hashedpassword", UserRole.User);
            user.Id = userId; // Set ID for the mock user

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userEmail)
            };
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"));

            _mockAuthService.Setup(s => s.GetUserByEmailAsync(userEmail)).ReturnsAsync(user);

            // Act
            var result = await _controller.DownloadUserData();

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/json", fileResult.ContentType);
            Assert.Equal($"user_data_{userId}.json", fileResult.FileDownloadName);

            var jsonContent = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);
            var deserializedData = JsonSerializer.Deserialize<JsonElement>(jsonContent);
            Assert.Equal(userId, deserializedData.GetProperty("Id").GetInt32());
            Assert.Equal("Test User", deserializedData.GetProperty("Name").GetString());
            Assert.Equal(userEmail, deserializedData.GetProperty("Email").GetString());
            Assert.Equal(UserRole.User.ToString(), deserializedData.GetProperty("Role").GetString());

            _mockAuthService.Verify(s => s.GetUserByEmailAsync(userEmail), Times.Once);
        }

        [Fact]
        public void DeleteAccountConfirmation_GET_UserNotAuthenticated_RedirectsToLogin()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // Not authenticated

            var result = _controller.DeleteAccountConfirmation();

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectToActionResult.ActionName);
        }

        [Fact]
        public void DeleteAccountConfirmation_GET_UserAuthenticated_ReturnsViewResult()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")); // Authenticated

            var result = _controller.DeleteAccountConfirmation();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task DeleteAccount_POST_UserNotAuthenticated_ReturnsUnauthorized()
        {
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // Not authenticated

            var result = await _controller.DeleteAccount();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task DeleteAccount_POST_UserAuthenticated_SuccessfulDeletion_RedirectsToHomeIndex()
        {
            var userId = 1;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")); // Authenticated

            _mockAuthService.Setup(s => s.DeleteAccountAsync(userId)).Returns(Task.CompletedTask);
            _mockAuthService.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAccount();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            _mockAuthService.Verify(s => s.DeleteAccountAsync(userId), Times.Once);
            _mockAuthService.Verify(s => s.SignOutAsync(), Times.Once);
        }

        [Fact]
        public void AccessDenied_GET_ReturnsViewResult()
        {
            var result = _controller.AccessDenied();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ExternalLogin_GET_ReturnsChallengeResult()
        {
            var provider = "Google";
            var returnUrl = "/";
            _mockAuthService.Setup(s => s.ConfigureExternalAuthenticationProperties(provider, It.IsAny<string>()))
                .Returns(new AuthenticationProperties { RedirectUri = "/Account/ExternalLoginCallback?returnUrl=/" });

            // Act
            var result = _controller.ExternalLogin(provider, returnUrl);

            // Assert
            var challengeResult = Assert.IsType<ChallengeResult>(result);
            Assert.Equal(provider, challengeResult.AuthenticationSchemes.Single());
            _mockAuthService.Verify(s => s.ConfigureExternalAuthenticationProperties(provider, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ExternalLoginCallback_GET_SuccessfulExternalLogin_RedirectsToLocal()
        {
            var returnUrl = "/";
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") }, "Cookies"));
            _mockAuthService.Setup(s => s.ExternalLoginCallbackAsync(returnUrl, null))
                .ReturnsAsync((claimsPrincipal, new Dictionary<string, string>()));

            // Act
            var result = await _controller.ExternalLoginCallback(returnUrl, null);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            _mockAuthService.Verify(s => s.ExternalLoginCallbackAsync(returnUrl, null), Times.Once);
        }

        [Fact]
        public async Task ExternalLoginCallback_GET_FailedExternalLogin_ReturnsViewWithModelError()
        {
            var returnUrl = "/";
            var remoteError = "Error from provider";
            var errors = new Dictionary<string, string> { { string.Empty, $"Error from external provider: {remoteError}" } };
            _mockAuthService.Setup(s => s.ExternalLoginCallbackAsync(returnUrl, remoteError))
                .ReturnsAsync((null, errors));

            // Act
            var result = await _controller.ExternalLoginCallback(returnUrl, remoteError);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Login", viewResult.ViewName); // Corrected to use literal string "Login"
            Assert.False(_controller.ModelState.IsValid);
            Assert.Contains($"Error from external provider: {remoteError}", _controller.ModelState[string.Empty]!.Errors.Select(e => e.ErrorMessage));
            _mockAuthService.Verify(s => s.ExternalLoginCallbackAsync(returnUrl, remoteError), Times.Once);
        }
    }
}
