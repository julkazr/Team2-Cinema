using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class Levi9PaymentControllerTests
    {
        private Mock<ILevi9PaymentService> _paymentService;

        [TestMethod]
        public void Post_Return_Ok()
        {
            //Arrange
            PaymentResponse paymentResponse = new PaymentResponse
            {
                IsSuccess = true,
                Message = "Payment is successful."
            };
            PaymentResponseModel paymentResponseModel = new PaymentResponseModel
            {
                IsSuccess = paymentResponse.IsSuccess,
                Message = paymentResponse.Message
            };
            _paymentService = new Mock<ILevi9PaymentService>();
            _paymentService.Setup(x => x.MakePayment()).Returns(Task.Run(() => {
                return paymentResponse;
            }));
            int expectedStatusCode = 200;

            Levi9PaymentController paymentController = new Levi9PaymentController(_paymentService.Object);

            //Act
            var result = paymentController.Post()
                                          .ConfigureAwait(false)
                                          .GetAwaiter()
                                          .GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var paymentResult = (PaymentResponseModel)resultList;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.AreEqual(paymentResponse.Message, paymentResult.Message);
            Assert.AreEqual(paymentResponse.IsSuccess, paymentResult.IsSuccess);
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void Post_UnSuccessful_Return_BadRequest()
        {
            //Arrange
            PaymentResponse paymentResponse = new PaymentResponse
            {
                IsSuccess = false,
                Message = "Insufficient founds."
            };
            PaymentResponseModel paymentResponseModel = new PaymentResponseModel
            {
                IsSuccess = paymentResponse.IsSuccess,
                Message = paymentResponse.Message
            };
            _paymentService = new Mock<ILevi9PaymentService>();
            _paymentService.Setup(x => x.MakePayment()).Returns(Task.Run(() => {
                return paymentResponse;
            }));
            int expectedStatusCode = 400;

            Levi9PaymentController paymentController = new Levi9PaymentController(_paymentService.Object);

            //Act
            var result = paymentController.Post()
                                          .ConfigureAwait(false)
                                          .GetAwaiter()
                                          .GetResult().Result;
            var resultList = ((BadRequestObjectResult)result).Value;
            var paymentResult = (PaymentResponseModel)resultList;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.AreEqual(paymentResponse.Message, paymentResult.Message);
            Assert.AreEqual(paymentResponse.IsSuccess, paymentResult.IsSuccess);
            Assert.AreEqual(expectedStatusCode, ((BadRequestObjectResult)result).StatusCode);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void Post_ConnectionError_Return_BadRequest()
        {
            //Arrange
            PaymentResponse paymentResponse = new PaymentResponse
            {
                IsSuccess = false,
                Message = "Connection error."
            };
            PaymentResponseModel paymentResponseModel = new PaymentResponseModel
            {
                IsSuccess = paymentResponse.IsSuccess,
                Message = paymentResponse.Message
            };
            ErrorResponseModel errorResponse = new ErrorResponseModel
            {
                ErrorMessage = Messages.PAYMENT_CREATION_ERROR,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };

            _paymentService = new Mock<ILevi9PaymentService>();
            _paymentService.Setup(x => x.MakePayment()).Returns(Task.Run(() => {
                return paymentResponse;
            }));
            int expectedStatusCode = 400;

            Levi9PaymentController paymentController = new Levi9PaymentController(_paymentService.Object);

            //Act
            var result = paymentController.Post()
                                          .ConfigureAwait(false)
                                          .GetAwaiter()
                                          .GetResult().Result;
            var resultList = ((BadRequestObjectResult)result).Value;
            var paymentResult = (ErrorResponseModel)resultList;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.AreEqual(errorResponse.ErrorMessage, paymentResult.ErrorMessage);
            Assert.AreEqual(expectedStatusCode, ((BadRequestObjectResult)result).StatusCode);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}
