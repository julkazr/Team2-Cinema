using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class CinemasControllerTests
    {
        private Mock<ICinemaService> _cinemaService;
        private Mock<IAuditoriumService> _auditoriumService;

        CreateCinemaWithAuditoriumModel createCinemaWith;
        CinemaDomainModel cinemaDomain;

        AuditoriumDomainModel auditoriumDomainModel;
        CreateAuditoriumResultModel createAuditorium;


        [TestInitialize]
        public void TestInitialize()
        {
            cinemaDomain = new CinemaDomainModel
            {
                Id = 1,
                Name = "cinemaName"
            };

            SeatDomainModel seatDomainModel = new SeatDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Number = 1,
                Row = 1
            };

            auditoriumDomainModel = new AuditoriumDomainModel
            {
                Id = 1,
                Name = "auditName",
                CinemaId = 1,
                SeatsList = new List<SeatDomainModel>()
            };
            auditoriumDomainModel.SeatsList.Add(seatDomainModel);
            createAuditorium = new CreateAuditoriumResultModel
            {
                Auditorium = auditoriumDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };

            createCinemaWith = new CreateCinemaWithAuditoriumModel
            {
                auditName = "auditName",
                cinemaName = "cinemaName",
                numberOfSeats = 1,
                seatRows = 1
            };

            _cinemaService = new Mock<ICinemaService>();
            _auditoriumService = new Mock<IAuditoriumService>();
            _cinemaService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Returns(Task.FromResult(cinemaDomain));
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(createAuditorium));
        }

        [TestMethod]
        public void GetAsync_Return_AllCinemas()
        {
            //Arrange
            List<CinemaDomainModel> cinemaDomainModels = new List<CinemaDomainModel>();
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "NameOfCinema"
            };
            cinemaDomainModels.Add(cinemaDomainModel);
            IEnumerable<CinemaDomainModel> domainModels = cinemaDomainModels;
            Task<IEnumerable<CinemaDomainModel>> responseTask = Task.FromResult(domainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResultList = (List<CinemaDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResultList);
            Assert.AreEqual(expectedResultCount, cinemaDomainModelResultList.Count);
            Assert.AreEqual(cinemaDomainModel.Id, cinemaDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsync_Return_NewList()
        {
            //Arrange
            IEnumerable<CinemaDomainModel> cinemaDomainModels = null;
            Task<IEnumerable<CinemaDomainModel>> responseTask = Task.FromResult(cinemaDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResultList = (List<CinemaDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResultList);
            Assert.AreEqual(expectedResultCount, cinemaDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetById_Return_Cinema()
        {
            //Arrange
            int id = 1;
            int expectedStatusCode = 200;
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "NameOfCinema"
            };
            var responseTask = Task.FromResult(cinemaDomainModel);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetByIdAsync(id)).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.GetByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var cinemaDomainModelResult = (CinemaDomainModel)resultList;

            //Assert
            Assert.IsNotNull(cinemaDomainModelResult);
            Assert.AreEqual(cinemaDomainModel.Id, cinemaDomainModelResult.Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetById_Return_NotFound()
        {
            //Arrange
            int id = 1;
            int expectedStatusCode = 404;
            string message = Messages.CINEMA_DOES_NOT_EXIST;
            CinemaDomainModel cinemaDomainModel = null;
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetByIdAsync(id)).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.GetByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultMessage = ((ObjectResult)result).Value;

            //Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)result).StatusCode);
            Assert.AreEqual(message, resultMessage);
        }

        [TestMethod]
        public void Post_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            CinemaModel createCinemaModel = new CinemaModel()
            {
                Name = null
            };

            _cinemaService = new Mock<ICinemaService>();
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);
            cinemasController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = cinemasController.Post(createCinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void PostAsync_Create_createCinemaResultModel_Return_CreatedCinema()
        {
            //Arrange
            int expectedStatusCode = 201;
            CinemaModel createCinemaModel = new CinemaModel
            {
                Name = "NameOfCinema"
            };
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = createCinemaModel.Name
            };
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.Post(createCinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var createdResult = ((CreatedResult)result).Value;
            var domainModel = (CinemaDomainModel)createdResult;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(cinemaDomainModel.Id, domainModel.Id);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);

        }

        [TestMethod]
        public void Post_Create_Throw_DbException_Cinema()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            CinemaModel createCinemaModel = new CinemaModel
            {
                Name = "NameOfCinema"
            };
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = createCinemaModel.Name
            };
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Throws(dbUpdateException);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.Post(createCinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void Post_Create_createCinema_IsNull_True_Return_InternalServerError()
        {
            //Arrange
            string expectedMessage = Messages.CINEMA_CREATION_ERROR;
            int expectedStatusCode = 500;

            CinemaModel createCinemaModel = new CinemaModel
            {
                Name = "NameOfCinema"
            };
            CinemaDomainModel cinemaDomainModel = null;
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.Post(createCinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void Put_UpdateCinema_ReturnAccepted()
        {
            //Arrange 
            int expectedStatusCode = 202;
            int id = 1;
            CinemaModel cinemaModel = new CinemaModel
            {
                Name = "NameOfCinema"
            };
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = id,
                Name = "OldNameOfCinema"
            };
            CinemaDomainModel newCinemaDomainModel = new CinemaDomainModel
            {
                Id = id,
                Name = "NameOfCinema"
            };
            Task<CinemaDomainModel> responseCinema = Task.FromResult(cinemaDomainModel);
            Task<CinemaDomainModel> responseTask = Task.FromResult(newCinemaDomainModel);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseCinema);
            _cinemaService.Setup(x => x.UpdateCinema(It.IsAny<CinemaDomainModel>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.Put(id, cinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((ObjectResult)result).Value;
            var domainModel = (CinemaDomainModel)objectResult;
            var resultResponse = (ObjectResult)result;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(newCinemaDomainModel.Id, domainModel.Id);
            Assert.AreEqual(newCinemaDomainModel.Name, domainModel.Name);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Put_GetByIdReturnNull_Return_BadRequest()
        {
            //Arrange 
            int expectedStatusCode = 400;
            string expectedMessage = Messages.CINEMA_DOES_NOT_EXIST;
            int id = 1;
            CinemaModel cinemaModel = new CinemaModel
            {
                Name = "NameOfCinema"
            };
            CinemaDomainModel cinemaDomainModel = null;
            Task<CinemaDomainModel> responseCinema = Task.FromResult(cinemaDomainModel);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseCinema);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.Put(id, cinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void Put_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            int id = 1;
            CinemaModel cinemaModel = new CinemaModel
            {
                Name = null
            };

            _cinemaService = new Mock<ICinemaService>();
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);
            cinemasController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = cinemasController.Put(id, cinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void Put_Update_Throw_DbException_Cinema()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            int id = 1;
            CinemaModel createCinemaModel = new CinemaModel
            {
                Name = "NameOfCinema"
            };
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "OldNameOfCinema"
            };
            CinemaDomainModel newCinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "NameOfCinema"
            };
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            Task<CinemaDomainModel> responseCinema = Task.FromResult(cinemaDomainModel);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseCinema);
            _cinemaService.Setup(x => x.UpdateCinema(It.IsAny<CinemaDomainModel>())).Throws(dbUpdateException);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.Put(id, createCinemaModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void Delete_DeleteCinema_ReturnA_Accepted()
        {
            int expectedStatusCode = 202;
            int id = 1;
            CinemaDomainModel cinemaDomainModel = new CinemaDomainModel
            {
                Id = id,
                Name = "NameOfCinema"
            };
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((ObjectResult)result).Value;
            var domainModel = (CinemaDomainModel)objectResult;
            var resultResponse = (ObjectResult)result;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(cinemaDomainModel.Id, domainModel.Id);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Delete_Delete_Throw_DbException_Cinema()
        {
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            int id = 1;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Throws(dbUpdateException);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
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
            string expectedMessage = Messages.CINEMA_DOES_NOT_EXIST;
            int expectedStatusCode = 500;
            int id = 1;
            CinemaDomainModel cinemaDomainModel = null;
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);

            _cinemaService = new Mock<ICinemaService>();
            _cinemaService.Setup(x => x.DeleteCinema(It.IsAny<int>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void CinemasController_PostWithAuditorium_ReturnCreatedCinemaAndAuditorium()
        {
            //Arrange
            int expectedCode = 201;
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var actionResult = cinemasController.PostCreateCinemaWithAuditorium(createCinemaWith).ConfigureAwait(false).GetAwaiter().GetResult();
            var createdResult = ((CreatedResult)actionResult).Value;
            var domainModel = (CinemaDomainModel)createdResult;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(cinemaDomain.Id, domainModel.Id);
            Assert.IsInstanceOfType(actionResult, typeof(CreatedResult));
            Assert.AreEqual(expectedCode, ((CreatedResult)actionResult).StatusCode);
        }

        [TestMethod]
        public void CinemasController_PostWithAuditorium_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);
            cinemasController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var actionResult = cinemasController.PostCreateCinemaWithAuditorium(createCinemaWith).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)actionResult;
            var createdResult = ((BadRequestObjectResult)actionResult).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);

        }

        [TestMethod]
        public void CinemasController_PostWithAuditorium_Throw_DbException_Cinema()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _cinemaService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Throws(dbUpdateException);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.PostCreateCinemaWithAuditorium(createCinemaWith).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void CinemasController_PostWithAuditorium_AddCinemaReturnNull_ReturnErrorResponse()
        {
            //Arrange
            string expectedMessage = Messages.CINEMA_CREATION_ERROR;
            int expectedStatusCode = 500;
            CinemaDomainModel cinemaDomainModel = null;
            Task<CinemaDomainModel> responseTask = Task.FromResult(cinemaDomainModel);

            _cinemaService.Setup(x => x.AddCinema(It.IsAny<CinemaDomainModel>())).Returns(responseTask);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.PostCreateCinemaWithAuditorium(createCinemaWith).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void CinemasController_PostWithAuditorium_Throw_DbException_Auditorium()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Throws(dbUpdateException);
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.PostCreateCinemaWithAuditorium(createCinemaWith).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void CinemasController_PostWithAuditorium_CreateAuditoriumIsNotSuccessful_ReturnErrorResponse()
        {
            //Arrange
            string expectedMessage = Messages.AUDITORIUM_SAME_NAME;
            int expectedStatusCode = 400;
            createAuditorium = new CreateAuditoriumResultModel
            {
                ErrorMessage = Messages.AUDITORIUM_SAME_NAME,
                IsSuccessful = false
            };

            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(createAuditorium));
            CinemasController cinemasController = new CinemasController(_cinemaService.Object, _auditoriumService.Object);

            //Act
            var result = cinemasController.PostCreateCinemaWithAuditorium(createCinemaWith).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (ObjectResult)result;
            var ObjectResult = ((ObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)ObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }
    }
}
