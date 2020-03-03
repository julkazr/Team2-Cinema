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
using WinterWorkShop.Cinema.Domain.Services;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class ReservationsControllerTests
    {
        private Mock<IReservationService> _reservationService;
        private Mock<ILevi9PaymentService> _levi9PaymentService;
        private Mock<IUserService> _userService;

        ReservationDomainModel reservationDomainModel;
        ReservationModel reservationModel;
        ReservationProcesModel reservationProces;
        CheckSeetPositionsModel check;

        [TestInitialize]
        public void TestInitialize()
        {
            reservationDomainModel = new ReservationDomainModel
            {
                id = 1,
                projectionId = Guid.NewGuid(),
                seatId = Guid.NewGuid(),
                userId = Guid.NewGuid()
            };
            List<ReservationDomainModel> reservationDomainModels = new List<ReservationDomainModel>();
            reservationDomainModels.Add(reservationDomainModel);
            IEnumerable<ReservationDomainModel> domainModels = reservationDomainModels;
            Task<IEnumerable<ReservationDomainModel>> responseTask = Task.FromResult(domainModels);
            reservationModel = new ReservationModel
            {
                projectionId = reservationDomainModel.projectionId,
                seatId = reservationDomainModel.seatId,
                userId = reservationDomainModel.userId
            };
            reservationProces = new ReservationProcesModel
            {
                ProjectionId = reservationModel.projectionId,
                UserId = reservationModel.userId
            };
            reservationProces.SeatsToReserveID = new List<Guid>();
            reservationProces.SeatsToReserveID.Add(reservationDomainModel.seatId);
            CheckReservationForSeatsDomainModel checkReservation = new CheckReservationForSeatsDomainModel
            {
                SeatsAreFree = true,
                SeatsTaken = new List<Guid>()
            };
            checkReservation.SeatsTaken.Add(reservationDomainModel.seatId);
            check = new CheckSeetPositionsModel
            {
                listOfSeatsId = new List<Guid>()
            };
            check.listOfSeatsId.Add(reservationDomainModel.seatId);
            CheckSeatsPositionDomainModel model = new CheckSeatsPositionDomainModel
            {
                CheckSucceed = true,
                InfoMessage = "You passed only one seet"
            };
            UserDomainModel userDomainModel = new UserDomainModel
            {
                bonus = 1,
                Id = Guid.NewGuid(),
                FirstName = "fN",
                IsAdmin = true,
                LastName = "lN",
                UserName = "uN"
            };

            PaymentResponse paymentResponse = new PaymentResponse
            {
                IsSuccess = true
            };
            Task<PaymentResponse> responsePayment = Task.FromResult(paymentResponse); 

            _reservationService = new Mock<IReservationService>();
            _levi9PaymentService = new Mock<ILevi9PaymentService>();
            _userService = new Mock<IUserService>();
            _reservationService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            _reservationService.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(reservationDomainModel));
            _reservationService.Setup(x => x.AddReservation(It.IsAny<ReservationDomainModel>())).Returns(Task.FromResult(reservationDomainModel));
            _reservationService.Setup(x => x.CheckReservationForSeatsForProjection(It.IsAny<List<Guid>>(), It.IsAny<Guid>())).Returns(Task.FromResult(checkReservation));
            _reservationService.Setup(x => x.CheckPositionBeforeReservation(It.IsAny<List<Guid>>())).Returns(Task.FromResult(model));
            _levi9PaymentService.Setup(x => x.MakePayment()).Returns(responsePayment);
            _userService.Setup(x => x.IncreaseBonus(It.IsAny<Guid>(), It.IsAny<int>())).Returns(Task.FromResult(userDomainModel));
        }

        [TestMethod]
        public void ReservationsController_GetAll_ReturnAllReservation()
        {
            //Arrange
            int expectedCount = 1;
            int expectedCode = 200;
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.getAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var result = (List< ReservationDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(reservationDomainModel.id, result[0].id);
            Assert.AreEqual(expectedCode, ((OkObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
        }

        [TestMethod]
        public void ReservationsController_GetAllReturnNull_ReturnNewList()
        {
            //Arrange
            int expectedCount = 0;
            int expectedCode = 200;
            IEnumerable<ReservationDomainModel> domainModels = null;
            Task<IEnumerable<ReservationDomainModel>> responseTask = Task.FromResult(domainModels);
            _reservationService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.getAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var result = (List<ReservationDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(expectedCode, ((OkObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
        }

        [TestMethod]
        public void ReservationsController_GetById_ReturnReservationById()
        {
            //Arrange
            int id = 1;
            int expectedCode = 200;
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.getByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var result = (ReservationDomainModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(reservationDomainModel.id, result.id);
            Assert.AreEqual(expectedCode, ((OkObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
        }

        [TestMethod]
        public void ReservationsController_GetByIdReturnNull_ReturnNotFound()
        {
            //Arrange
            int id = 1;
            int expectedCode = 404;
            string expectedMessage = Messages.RESERVATION_DOES_NOT_EXIST;
            _reservationService.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult((ReservationDomainModel)null));
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.getByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)actionResult).Value;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.AreEqual(expectedMessage, resultList);
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
        }

        [TestMethod]
        public void ReservationsController_Post_ReturnCreatedReservation()
        {
            //Arrange
            int expectedCode = 201;
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.Post(reservationModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (ReservationDomainModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(reservationDomainModel.id, result.id);
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
        }

        [TestMethod]
        public void ReservationsController_Post_AddReturnNull_ReturnInternalServerError()
        {
            //Arrange
            int expectedCode = 500;
            string expectedMessages = Messages.RESERVATION_CREATION_ERROR;
            _reservationService.Setup(x => x.AddReservation(It.IsAny<ReservationDomainModel>())).Returns(Task.FromResult((ReservationDomainModel)null));
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.Post(reservationModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (ErrorResponseModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
            Assert.AreEqual(expectedMessages, result.ErrorMessage);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
        }

        [TestMethod]
        public void ReservationController_Post_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            _reservationService = new Mock<IReservationService>();
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);
            reservationsController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var actionResult = reservationsController.Post(reservationModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void ReservationController_Post_Create_Throw_DbException_Reservation()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _reservationService.Setup(x => x.AddReservation(It.IsAny<ReservationDomainModel>())).Throws(dbUpdateException);
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var result = reservationsController.Post(reservationModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void ReservationController_ReservationProces_ReturnCreated()
        {
            //Arrange
            int expectedCode = 201;
            int expectedCount = 1;
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.ReservationProces(reservationProces).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (List<ReservationDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(reservationDomainModel.id, result[0].id);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
        }

        [TestMethod]
        public void ReservationController_ReservationProces_WithUnValidState_ReturnBadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);
            reservationsController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var actionResult = reservationsController.ReservationProces(reservationProces).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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
        public void ReservationController_ReservationProces_SeatsNotFree_ReturnBadRequest()
        {
            //Arrange
            int expectedCode = 400;
            string expectedMessages = "Some of seats are already reserved";
            CheckReservationForSeatsDomainModel checkReservation = new CheckReservationForSeatsDomainModel
            {
                SeatsAreFree = false,
                InfoMessage = "Some of seats are already reserved"
            };
            _reservationService.Setup(x => x.CheckReservationForSeatsForProjection(It.IsAny<List<Guid>>(), It.IsAny<Guid>())).Returns(Task.FromResult(checkReservation));
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.ReservationProces(reservationProces).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (SeatTakenErrorResponseModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessages, result.ErrorMessage);
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
        }

        [TestMethod]
        public void ReservationController_ReservationProces_PaymentIsNotSuccess_ReturnBadRequest()
        {
            //Arrange
            int expectedCode = 400;
            string expectedMessage = "Connection error.";
            PaymentResponse paymentResponse = new PaymentResponse
            {
                IsSuccess = false,
                Message = "Connection error."
            };
            Task<PaymentResponse> responsePayment = Task.FromResult(paymentResponse);
            _levi9PaymentService.Setup(x => x.MakePayment()).Returns(responsePayment);
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.ReservationProces(reservationProces).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (ErrorResponseModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
        }

        [TestMethod]
        public void ReservationController_ReservationProces_Throw_DbException_Reservation()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            _reservationService.Setup(x => x.AddReservation(It.IsAny<ReservationDomainModel>())).Throws(dbUpdateException);
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var result = reservationsController.Post(reservationModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void ReservationController_Check_ReturnOk()
        {
            //Arrange 
            int expectedCode = 200;
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.CheckSeatPositions(check).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var result = (CheckSeatsPositionDomainModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCode, ((OkObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
        }

        [TestMethod]
        public void ReservationController_Check_WithUnValidState_ReturnBadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);
            reservationsController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var actionResult = reservationsController.ReservationProces(reservationProces).ConfigureAwait(false).GetAwaiter().GetResult().Result;
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
        public void ReservationController_Check_ListOfIdSeatIsEmpty_ReturnBadRequest()
        {
            //Arrange 
            int expectedCode = 400;
            string expectedMessage = "List of id-seats is not valid";
            check = new CheckSeetPositionsModel
            {
                listOfSeatsId = new List<Guid>()
            };
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.CheckSeatPositions(check).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (ErrorResponseModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
        }

        [TestMethod]
        public void ReservationController_CheckReturnNull_ReturnBadRequest()
        {
            //Arrange 
            int expectedCode = 400;
            string expectedMessage = "Something went wrong in service";
            CheckSeatsPositionDomainModel model = null;
            _reservationService.Setup(x => x.CheckPositionBeforeReservation(It.IsAny<List<Guid>>())).Returns(Task.FromResult(model));
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.CheckSeatPositions(check).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (ErrorResponseModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
        }

        [TestMethod]
        public void ReservationController_CheckIsNotSucced_ReturnBadRequest()
        {
            //Arrange 
            int expectedCode = 400;
            string expectedMessage = "Seets are not next to each other and exceeding the row";
            CheckSeatsPositionDomainModel model = new CheckSeatsPositionDomainModel
            {
                CheckSucceed = false,
                InfoMessage = "Seets are not next to each other and exceeding the row"
            };
            _reservationService.Setup(x => x.CheckPositionBeforeReservation(It.IsAny<List<Guid>>())).Returns(Task.FromResult(model));
            ReservationsController reservationsController = new ReservationsController(_reservationService.Object, _levi9PaymentService.Object, _userService.Object);

            //Act
            var actionResult = reservationsController.CheckSeatPositions(check).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)actionResult).Value;
            var result = (ErrorResponseModel)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCode, ((ObjectResult)actionResult).StatusCode);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(actionResult, typeof(ObjectResult));
        }
    }
}
