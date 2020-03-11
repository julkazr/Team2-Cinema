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
    public class AuditoriumsControllerTests
    {
        private Mock<IAuditoriumService> _auditoriumService;
        private Mock<IReservationService> _reservationService;
        AuditoriumsController auditoriumsController;
        AuditoriumDomainModel auditoriumDomainModel;
        List<AuditoriumDomainModel> _auditoriumDomainModels;
        CheckReservationForSeatsDomainModel check;
        CreateAuditoriumModel create;
        CreateAuditoriumResultModel createAuditoriumResultModel;
        UpdateAuditoriumModel update;

        [TestInitialize]
        public void TestInitialize()
        {
            SeatDomainModel seat = new SeatDomainModel
            {
                AuditoriumId = 1,
                Id = Guid.NewGuid(),
                Number = 1,
                Row = 1
            };
            _auditoriumDomainModels = new List<AuditoriumDomainModel>();
            auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = 1,
                Id = 1,
                Name = "auditName",
                SeatsList = new List<SeatDomainModel>()
            };
            auditoriumDomainModel.SeatsList.Add(seat);
            _auditoriumDomainModels.Add(auditoriumDomainModel);
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels = _auditoriumDomainModels;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriumDomainModels);
            createAuditoriumResultModel = new CreateAuditoriumResultModel
            {
                Auditorium = auditoriumDomainModel,
                ErrorMessage = null,
                IsSuccessful = true
            };
            create = new CreateAuditoriumModel
            {
                auditName = auditoriumDomainModel.Name,
                cinemaId = auditoriumDomainModel.CinemaId,
                numberOfSeats = 1,
                seatRows = 1
            };
            update = new UpdateAuditoriumModel
            {
                Name = auditoriumDomainModel.Name,
                NumberOfRows = 1,
                NumberOfSeats = 1
            };

            check = new CheckReservationForSeatsDomainModel
            {
                SeatsAreFree = true
            };

            _auditoriumService = new Mock<IAuditoriumService>();
            _reservationService = new Mock<IReservationService>();
            _auditoriumService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            _auditoriumService.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(auditoriumDomainModel));
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(createAuditoriumResultModel));
            _auditoriumService.Setup(x => x.UpdateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.FromResult(auditoriumDomainModel));
            _auditoriumService.Setup(x => x.DeleteAuditorium(It.IsAny<int>())).Returns(Task.FromResult(auditoriumDomainModel));
            _reservationService.Setup(x => x.CheckReservationForSeats(It.IsAny<List<Guid>>())).Returns(Task.FromResult(check));
            auditoriumsController = new AuditoriumsController(_auditoriumService.Object, _reservationService.Object);

        }

        [TestMethod]
        public void AuditoriumsController_GetAll_ReturnAllAuditoriums()
        {
            //Arrange 
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            //Act
            var actionResult = auditoriumsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var result = (List<AuditoriumDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(auditoriumDomainModel.Id, result[0].Id);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)actionResult).StatusCode);

        }

        [TestMethod]
        public void AuditoriumsController_GetAllReturnNull_ReturnNewList()
        {
            //Arrange 
            IEnumerable<AuditoriumDomainModel> auditoriumDomainModels = null;
            Task<IEnumerable<AuditoriumDomainModel>> responseTask = Task.FromResult(auditoriumDomainModels);
            _auditoriumService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            auditoriumsController = new AuditoriumsController(_auditoriumService.Object, _reservationService.Object);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            //Act
            var actionResult = auditoriumsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var result = (List<AuditoriumDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)actionResult).StatusCode);

        }

        [TestMethod]
        public void AuditoriumsController_GetById_ReturnAuditoriumById()
        {
            //Arrange
            int id = 1;
            int expectedStatusCode = 200;

            //Act
            var actionResult = auditoriumsController.GetAsync(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var result = (AuditoriumDomainModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(auditoriumDomainModel.Id, result.Id);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)actionResult).StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_GetByIdReturnNull_ReturnNotFound()
        {
            //Arrange
            int id = 1;
            int expectedStatusCode = 404;
            string expectedMessage = Messages.AUDITORIUM_DOES_NOT_EXIST;
            _auditoriumService.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult((AuditoriumDomainModel)null));
            auditoriumsController = new AuditoriumsController(_auditoriumService.Object, _reservationService.Object);

            //Act
            var actionResult = auditoriumsController.GetAsync(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)actionResult).Value;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
            Assert.AreEqual(expectedMessage, resultList);
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)actionResult).StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Post_ReturnCreatedAuditorium()
        {
            //Arrange
            int expectedCode = 201;
            int row = 1;
            int numb = 1;

            //Act
            var actionResult = auditoriumsController.PostAsync(create).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (CreateAuditoriumResultModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(auditoriumDomainModel.Id, result.Auditorium.Id);
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOfType(result, typeof(CreateAuditoriumResultModel));
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Post_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            auditoriumsController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var actionResult = auditoriumsController.PostAsync(create).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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
        public void AuditoriumsController_Post_Create_Throw_DbException_Auditorium()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Throws(dbUpdateException);
            auditoriumsController = new AuditoriumsController(_auditoriumService.Object, _reservationService.Object);

            //Act
            var actionResult = auditoriumsController.PostAsync(create).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultResponse = (BadRequestObjectResult)actionResult;
            var badObjectResult = ((BadRequestObjectResult)actionResult).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Post_IsNotSuccessful_ReturnBadRequest()
        {
            //Arrange
            int expectedCode = 400;
            string expectedMessage = Messages.AUDITORIUM_CREATION_ERROR;
            createAuditoriumResultModel = new CreateAuditoriumResultModel
            {
                ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR,
                IsSuccessful = false
            };
            _auditoriumService.Setup(x => x.CreateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(createAuditoriumResultModel));
            _reservationService.Setup(x => x.CheckReservationForSeats(It.IsAny<List<Guid>>())).Returns(Task.FromResult(check));
            auditoriumsController = new AuditoriumsController(_auditoriumService.Object, _reservationService.Object);

            //Act
            var actionResult = auditoriumsController.PostAsync(create).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (ErrorResponseModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(ErrorResponseModel));
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Put_ReturnAccerted()
        {
            //Arrange
            int id = 1;
            int expectedCode = 202;

            //Act
            var actionResult = auditoriumsController.Put(id, update).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (AuditoriumDomainModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(auditoriumDomainModel.Id, result.Id);
            Assert.IsInstanceOfType(result, typeof(AuditoriumDomainModel));
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Put_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            int id = 1;
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            auditoriumsController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var actionResult = auditoriumsController.Put(id, update).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void AuditoriumsController_Put_Update_Throw_DbException_Auditorium()
        {
            //Arrange
            int id = 1;
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _auditoriumService.Setup(x => x.UpdateAuditorium(It.IsAny<AuditoriumDomainModel>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).Throws(dbUpdateException);
            auditoriumsController = new AuditoriumsController(_auditoriumService.Object, _reservationService.Object);

            //Act
            var actionResult = auditoriumsController.Put(id, update).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)actionResult;
            var badObjectResult = ((BadRequestObjectResult)actionResult).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Put_GetByIdReturnNull_ReturnBadRequest()
        {
            //Arrange
            int id = 1;
            string expectedMessage = Messages.AUDITORIUM_DOES_NOT_EXIST;
            int expectedCode = 400;
            _auditoriumService.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult((AuditoriumDomainModel)null));
            auditoriumsController = new AuditoriumsController(_auditoriumService.Object, _reservationService.Object);

            //Act
            var actionResult = auditoriumsController.Put(id, update).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)actionResult;
            var createdResult = ((BadRequestObjectResult)actionResult).Value;
            var result = (ErrorResponseModel)createdResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Delete_ReturnAccepted()
        {
            //Arrange
            int id = 1;
            int expectedCode = 202;

            //Act
            var actionResult = auditoriumsController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (AuditoriumDomainModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(auditoriumDomainModel.Id, result.Id);
            Assert.IsInstanceOfType(result, typeof(AuditoriumDomainModel));
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_Delete_Delete_Throw_DbException_Auditorium()
        {
            //Arrange
            int id = 1;
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _auditoriumService.Setup(x => x.DeleteAuditorium(It.IsAny<int>())).Throws(dbUpdateException);
            auditoriumsController = new AuditoriumsController(_auditoriumService.Object, _reservationService.Object);


            //Act
            var actionResult = auditoriumsController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)actionResult;
            var badObjectResult = ((BadRequestObjectResult)actionResult).Value;
            var errorResult = (ErrorResponseModel)badObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void AuditoriumsController_DeleteReturnNull_ReturnInternalServerError()
        {
            //Arrange
            int id = 1;
            int expectedCode = 500;
            string expectedMessage = Messages.AUDITORIUM_DOES_NOT_EXIST;
            _auditoriumService.Setup(x => x.DeleteAuditorium(It.IsAny<int>())).Returns(Task.FromResult((AuditoriumDomainModel)null));

            //Act
            var actionResult = auditoriumsController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (ObjectResult)actionResult;
            var createdResult = ((ObjectResult)actionResult).Value;
            var result = (ErrorResponseModel)createdResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
            Assert.AreEqual(expectedCode, resultResponse.StatusCode);
        }
    }
}
