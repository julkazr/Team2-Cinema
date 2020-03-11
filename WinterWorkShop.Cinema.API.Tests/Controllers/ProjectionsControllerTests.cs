using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class ProjectionsControllerTests
    {
        private Mock<IProjectionService> _projectionService;


        [TestMethod]
        public void GetAsync_Return_All_Projections()
        {
            //Arrange
            List<ProjectionDomainModel> projectionsDomainModelsList = new List<ProjectionDomainModel>();
            ProjectionDomainModel projectionDomainModel = new ProjectionDomainModel
            {
                Id = Guid.NewGuid(),
                AditoriumName = "ImeSale",
                AuditoriumId = 1,
                MovieId = Guid.NewGuid(),
                MovieTitle = "ImeFilma",
                ProjectionTime = DateTime.Now.AddDays(1)
            };
            projectionsDomainModelsList.Add(projectionDomainModel);
            IEnumerable<ProjectionDomainModel> projectionDomainModels = projectionsDomainModelsList;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var projectionDomainModelResultList = (List<ProjectionDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(projectionDomainModelResultList);
            Assert.AreEqual(expectedResultCount, projectionDomainModelResultList.Count);
            Assert.AreEqual(projectionDomainModel.Id, projectionDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            IEnumerable<ProjectionDomainModel> projectionDomainModels = null;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var projectionDomainModelResultList = (List<ProjectionDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(projectionDomainModelResultList);
            Assert.AreEqual(expectedResultCount, projectionDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - return valid mock
        // if (!createProjectionResultModel.IsSuccessful) - false
        // return Created
        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_True_Projection()
        {
            //Arrange
            int expectedStatusCode = 201;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            CreateProjectionResultModel createProjectionResultModel = new CreateProjectionResultModel
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AditoriumName = "ImeSale",
                    AuditoriumId = createProjectionModel.AuditoriumId,
                    MovieId = createProjectionModel.MovieId,
                    MovieTitle = "ImeFilma",
                    ProjectionTime = createProjectionModel.ProjectionTime
                },
                IsSuccessful = true
            };
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);


            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel)
                                              .ConfigureAwait(false)
                                              .GetAwaiter()
                                              .GetResult().Result;
            var createdResult = ((CreatedResult)result).Value;
            var projectionDomainModel = (ProjectionDomainModel)createdResult;

            //Assert
            Assert.IsNotNull(projectionDomainModel);
            Assert.AreEqual(createProjectionModel.MovieId, projectionDomainModel.MovieId);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - throw DbUpdateException
        // return BadRequest
        [TestMethod]
        public void PostAsync_Create_Throw_DbException_Projection()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            CreateProjectionResultModel createProjectionResultModel = new CreateProjectionResultModel
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AditoriumName = "ImeSale",
                    AuditoriumId = createProjectionModel.AuditoriumId,
                    MovieId = createProjectionModel.MovieId,
                    MovieTitle = "ImeFilma",
                    ProjectionTime = createProjectionModel.ProjectionTime
                },
                IsSuccessful = true
            };
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Throws(dbUpdateException);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }


        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - false
        // try  await _projectionService.CreateProjection(domainModel) - return valid mock
        // if (!createProjectionResultModel.IsSuccessful) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_Create_createProjectionResultModel_IsSuccessful_False_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Error occured while creating new projection, please try again.";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            CreateProjectionResultModel createProjectionResultModel = new CreateProjectionResultModel
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AditoriumName = "ImeSale",
                    AuditoriumId = createProjectionModel.AuditoriumId,
                    MovieId = createProjectionModel.MovieId,
                    MovieTitle = "ImeFilma",
                    ProjectionTime = createProjectionModel.ProjectionTime
                },
                IsSuccessful = false,
                ErrorMessage = Messages.PROJECTION_CREATION_ERROR,
            };
            Task<CreateProjectionResultModel> responseTask = Task.FromResult(createProjectionResultModel);


            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.CreateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        // if (!ModelState.IsValid) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(1),
                AuditoriumId = 0
            };

            _projectionService = new Mock<IProjectionService>();
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            projectionsController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        // if (!ModelState.IsValid) - false
        // if (projectionModel.ProjectionTime < DateTime.Now) - true
        // return BadRequest
        [TestMethod]
        public void PostAsync_With_UnValid_ProjectionDate_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Projection time cannot be in past.";
            int expectedStatusCode = 400;

            CreateProjectionModel createProjectionModel = new CreateProjectionModel()
            {
                MovieId = Guid.NewGuid(),
                ProjectionTime = DateTime.Now.AddDays(-1),
                AuditoriumId = 0
            };

            _projectionService = new Mock<IProjectionService>();
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.PostAsync(createProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault(nameof(createProjectionModel.ProjectionTime));
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void GetFiltered_Filtered_Return_Ok()
        {
            //Arrange
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
            FilterProjectionModel filterProjectionModel = new FilterProjectionModel
            {
                auditoriumId = 1,
                cinemaId = 1,
                fromTime = DateTime.Now,
                movieId = Guid.NewGuid(),
                toTime = DateTime.Now.AddDays(1)
            };
            List<ProjectionDomainModel> projectionDomainModels = new List<ProjectionDomainModel>();
            ProjectionDomainModel projectionDomainModel = new ProjectionDomainModel
            {
                AditoriumName = "auditName",
                AuditoriumId = 1,
                Id = Guid.NewGuid(),
                MovieId = Guid.NewGuid(),
                MovieTitle = "Title",
                ProjectionTime = DateTime.Now
            };
            projectionDomainModels.Add(projectionDomainModel);
            IEnumerable<ProjectionDomainModel> projectionDomains = projectionDomainModels;
            Task<IEnumerable<ProjectionDomainModel>> responseTask = Task.FromResult(projectionDomains);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.FilterProjections(It.IsAny<FilterProjectionDomainModel>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act 
            var result = projectionsController.GetFilteredProjections(filterProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<ProjectionDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void Put_UpdatedPtojection_Return_Accepted()
        {
            //Arrange
            int expectedStatusCode = 202;
            Guid id = Guid.NewGuid();
            UpdateProjectionModel updateProjectionModel = new UpdateProjectionModel
            {
                auditoriumId = 1,
                movieId = Guid.NewGuid(),
                projectionTime = DateTime.Now
            };
            ProjectionDomainModel projectionDomainModel = new ProjectionDomainModel
            {
                AditoriumName = "auditName",
                AuditoriumId = 1,
                Id = id,
                MovieId = Guid.NewGuid(),
                MovieTitle = "Title",
                ProjectionTime = DateTime.Now
            };
            ProjectionDomainModel projectionDomain = new ProjectionDomainModel
            {
                AditoriumName = projectionDomainModel.AditoriumName,
                AuditoriumId = 1,
                Id = id,
                MovieId = updateProjectionModel.movieId,
                MovieTitle = projectionDomainModel.MovieTitle,
                ProjectionTime = DateTime.Now
            };
            var responseProjection = Task.FromResult(projectionDomainModel);
            var responseTask = Task.FromResult(projectionDomain);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseProjection);
            _projectionService.Setup(x => x.UpdateProjection(It.IsAny<ProjectionDomainModel>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.Put(id, updateProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((ObjectResult)result).Value;
            var domainModel = (ProjectionDomainModel)objectResult;
            var resultResponse = (ObjectResult)result;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);

        }

        [TestMethod]
        public void Put_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            UpdateProjectionModel updateProjectionModel = null;
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            _projectionService = new Mock<IProjectionService>();
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);
            projectionsController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = projectionsController.Put(id, updateProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Put_GetByIdReturnNull_Return_BadRequest()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedMessage = Messages.PROJECTION_DOES_NOT_EXIST;
            Guid id = Guid.NewGuid();
            UpdateProjectionModel updateProjectionModel = new UpdateProjectionModel
            {
                auditoriumId = 1,
                movieId = Guid.NewGuid(),
                projectionTime = DateTime.Now
            };
            ProjectionDomainModel projectionDomainModel = null;
            var responseProjection = Task.FromResult(projectionDomainModel);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseProjection);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.Put(id, updateProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (ObjectResult)result;
            var ObjectResult = ((ObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)ObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Put_Create_Throw_DbException_Projection()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Guid id = Guid.NewGuid();
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            UpdateProjectionModel updateProjectionModel = new UpdateProjectionModel
            {
                auditoriumId = 1,
                movieId = Guid.NewGuid(),
                projectionTime = DateTime.Now
            };
            ProjectionDomainModel projectionDomainModel = new ProjectionDomainModel
            {
                AditoriumName = "auditName",
                AuditoriumId = 1,
                Id = id,
                MovieId = updateProjectionModel.movieId,
                MovieTitle = "Title",
                ProjectionTime = DateTime.Now
            }; ;
            var responseProjection = Task.FromResult(projectionDomainModel);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseProjection);
            _projectionService.Setup(x => x.UpdateProjection(It.IsAny<ProjectionDomainModel>())).Throws(dbUpdateException);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.Put(id, updateProjectionModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Delete_DeleteMovie_Return_Accepted()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            int expectedStatusCode = 202;
            ProjectionDomainModel projectionDomainModel = new ProjectionDomainModel
            {
                AditoriumName = "auditName",
                AuditoriumId = 1,
                Id = id,
                MovieId = Guid.NewGuid(),
                MovieTitle = "Title",
                ProjectionTime = DateTime.Now
            };
            var responseTask = Task.FromResult(projectionDomainModel);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.DeleteProjection(It.IsAny<Guid>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((ObjectResult)result).Value;
            var domainModel = (ProjectionDomainModel)objectResult;
            var resultResponse = (ObjectResult)result;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(projectionDomainModel.Id, domainModel.Id);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Delete_Delete_Throw_DbException_Movie()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.DeleteProjection(It.IsAny<Guid>())).Throws(dbUpdateException);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var badObjectResult = ((BadRequestObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Delete_DeleteReturnNull_Return_InternalServerError()
        {
            //Arrange
            string expectedMessage = "Error occured while deleting projection, please try again.";
            int expectedStatusCode = 500;
            Guid id = Guid.NewGuid();
            ProjectionDomainModel projectionDomainModel = null;
            var responseTask = Task.FromResult(projectionDomainModel);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.DeleteProjection(It.IsAny<Guid>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (ObjectResult)result;
            var ObjectResult = ((ObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)ObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void GetById_Get_Return_Ok()
        {
            Guid id = Guid.NewGuid();
            int expectedStatusCode = 200;
            ProjectionDomainModel projectionDomainModel = new ProjectionDomainModel
            {
                AditoriumName = "auditName",
                AuditoriumId = 1,
                Id = id,
                MovieId = Guid.NewGuid(),
                MovieTitle = "Title",
                ProjectionTime = DateTime.Now
            };
            var responseTask = Task.FromResult(projectionDomainModel);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.GetAsync(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultvalue = ((OkObjectResult)result).Value;
            var domainModel = (ProjectionDomainModel)resultvalue;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(projectionDomainModel.Id, domainModel.Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetById_GetReturnNull_Return_NotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            int expectedStatusCode = 404;
            string expectedMessages = "Projection does not exist.";
            ProjectionDomainModel projectionDomainModel = null;
            var responseTask = Task.FromResult(projectionDomainModel);

            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var result = projectionsController.GetAsync(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultvalue = ((ObjectResult)result).Value;

            //Assert
            Assert.IsNotNull(resultvalue);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)result).StatusCode);
            Assert.AreEqual(expectedMessages, resultvalue);
        }

        [TestMethod]
        public void ProjectionsController_GetAllReservad_ReturnOk()
        {
            //Arrange
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
            SeatDomainModel seat = new SeatDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Number = 1,
                Row = 1
            };
            List<SeatDomainModel> seatDomainModels = new List<SeatDomainModel>();
            seatDomainModels.Add(seat);
            IEnumerable<SeatDomainModel> seats = seatDomainModels;
            Task<IEnumerable<SeatDomainModel>> responseTask = Task.FromResult(seats);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetReserverdSeetsForProjection(It.IsAny<Guid>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var actionResult = projectionsController.GetAllReservedSeats(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var movieDomainModelResultList = (List<SeatDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)actionResult).StatusCode);
        }

        [TestMethod]
        public void ProjectionsController_GetAllReservadReturnNull_ReturnNewList()
        {
            //Arrange
            int expectedResultCount = 0;
            int expectedStatusCode = 200;
            IEnumerable<SeatDomainModel> seats = null;
            Task<IEnumerable<SeatDomainModel>> responseTask = Task.FromResult(seats);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetReserverdSeetsForProjection(It.IsAny<Guid>())).Returns(responseTask);
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var actionResult = projectionsController.GetAllReservedSeats(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var movieDomainModelResultList = (List<SeatDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)actionResult).StatusCode);
        }

        [TestMethod]
        public void ProjectionsController_GetWithAuditorium_ReturnOk()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            int expectedStatusCode = 200;
            var seats = new List<SeatDomainModel>();
            int maxNum = 0;
            int maxRow = 0;
            ProjectionWithAuditoriumResultModel projection = new ProjectionWithAuditoriumResultModel
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AditoriumName = "auditName",
                    AuditoriumId = 1,

                    MovieId = Guid.NewGuid(),
                    MovieTitle = "Title",
                    ProjectionTime = DateTime.Now
                },
                Auditorium = new AuditoriumDomainModel
                {
                    CinemaId = 1,
                    SeatsList = seats
                },
                Movie = new MovieDomainModel
                {
                    Rating = 7,
                    Year = 2020
                }
            };
            IEnumerable<SeatDomainModel> seatDomains = seats;
            Task<ProjectionWithAuditoriumResultModel> responseTask = Task.FromResult(projection);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetProjectionWithAuditorium(It.IsAny<Guid>())).Returns(responseTask);
            _projectionService.Setup(x => x.GetReserverdSeetsForProjection(It.IsAny<Guid>())).Returns(Task.FromResult(seatDomains));
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var actionResult = projectionsController.GetProjectionWithAuditorium(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((OkObjectResult)actionResult).Value;
            var projectionResultmodel = (ProjectionWithAuditoriumResultModel)result;

            //Assert
            Assert.IsNotNull(projectionResultmodel);
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)actionResult).StatusCode);
            Assert.AreEqual(projection.Projection.Id, projectionResultmodel.Projection.Id);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
        }

        [TestMethod]
        public void ProjectionsController_GetWithAuditorium_ReturnNotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            int expectedStatusCode = 404;
            string expectedMessages = "Projection does not exist.";
            ProjectionWithAuditoriumResultModel projection = null;
            //var seats = new List<SeatDomainModel>();
            List<SeatDomainModel> seats = null;
            IEnumerable<SeatDomainModel> seatDomains = seats;
            Task<ProjectionWithAuditoriumResultModel> responseTask = Task.FromResult(projection);
            _projectionService = new Mock<IProjectionService>();
            _projectionService.Setup(x => x.GetProjectionWithAuditorium(It.IsAny<Guid>())).Returns(responseTask);
            _projectionService.Setup(x => x.GetReserverdSeetsForProjection(It.IsAny<Guid>())).Returns(Task.FromResult(seatDomains));
            ProjectionsController projectionsController = new ProjectionsController(_projectionService.Object);

            //Act
            var actionResult = projectionsController.GetProjectionWithAuditorium(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var result = ((ObjectResult)actionResult).Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)actionResult).StatusCode);
            Assert.AreEqual(expectedMessages, result);
        }
    }
}
