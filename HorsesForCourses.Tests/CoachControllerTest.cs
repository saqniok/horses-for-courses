
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using HorsesForCourses.Core;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Xunit;

// public class CoachControllerTests
// {
//     private readonly Mock<ICoachService> _serviceMock;
//     private readonly CoachController _controller;

//     public CoachControllerTests()
//     {
//         _serviceMock = new Mock<ICoachService>();
//         _controller = new CoachController(_serviceMock.Object);
//     }

//     [Fact]
//     public void Add_ShouldReturnCreatedAtAction()
//     {
//         // Arrange
//         var dto = new CreateCoachDto { Name = "John", ExperienceYears = 5 };
//         var coach = new Coach("John", 5) { Id = 1 };

//         // Тут важно, чтобы маппер работал — иначе тесты надо писать с подменой маппера
//         _serviceMock.Setup(s => s.Create(It.IsAny<Coach>()))
//                     .Callback<Coach>(c => c.Id = 1);

//         // Act
//         var result = _controller.Add(dto) as CreatedAtActionResult;

//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(nameof(CoachController.GetById), result.ActionName);
//         Assert.Equal(1, ((CoachSummaryDto)result.Value).Id);
//     }

//     [Fact]
//     public void GetById_ShouldReturnOk_WhenCoachExists()
//     {
//         // Arrange
//         var coach = new Coach("John", 5) { Id = 2 };
//         _serviceMock.Setup(s => s.GetById(2)).Returns(coach);

//         // Act
//         var result = _controller.GetById(2) as OkObjectResult;

//         // Assert
//         Assert.NotNull(result);
//         var dto = result.Value as CoachDetailsDto;
//         Assert.NotNull(dto);
//         Assert.Equal("John", dto.Name);
//     }

//     [Fact]
//     public void GetById_ShouldReturnNotFound_WhenCoachDoesNotExist()
//     {
//         // Arrange
//         _serviceMock.Setup(s => s.GetById(99)).Returns((Coach?)null);

//         // Act
//         var result = _controller.GetById(99);

//         // Assert
//         Assert.IsType<NotFoundResult>(result.Result);
//     }

//     [Fact]
//     public void GetAll_ShouldReturnListOfCoaches()
//     {
//         // Arrange
//         var coaches = new List<Coach>
//         {
//             new Coach("John", 5) { Id = 1 },
//             new Coach("Jane", 3) { Id = 2 }
//         };
//         _serviceMock.Setup(s => s.GetAll()).Returns(coaches);

//         // Act
//         var result = _controller.GetAll() as OkObjectResult;

//         // Assert
//         Assert.NotNull(result);
//         var list = result.Value as IEnumerable<CoachSummaryDto>;
//         Assert.Equal(2, list.Count());
//     }

//     [Fact]
//     public void UpdateCoachSkills_ShouldReturnNoContent_WhenCoachExists()
//     {
//         // Arrange
//         var coach = new Coach("John", 5) { Id = 1 };
//         _serviceMock.Setup(s => s.GetById(1)).Returns(coach);
//         var dto = new UpdateCoachSkillsDto { Skills = new List<string> { "Strategy" } };

//         // Act
//         var result = _controller.UpdateCoachSkills(1, dto);

//         // Assert
//         Assert.IsType<NoContentResult>(result);
//         _serviceMock.Verify(s => s.Update(It.IsAny<Coach>()), Times.Once);
//     }

//     [Fact]
//     public void UpdateCoachSkills_ShouldReturnNotFound_WhenCoachDoesNotExist()
//     {
//         // Arrange
//         _serviceMock.Setup(s => s.GetById(5)).Returns((Coach?)null);
//         var dto = new UpdateCoachSkillsDto { Skills = new List<string> { "Bluffing" } };

//         // Act
//         var result = _controller.UpdateCoachSkills(5, dto);

//         // Assert
//         Assert.IsType<NotFoundResult>(result);
//         _serviceMock.Verify(s => s.Update(It.IsAny<Coach>()), Times.Never);
//     }
// }
