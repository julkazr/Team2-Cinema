using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Controllers;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class SeatsControllerTests
    {
        private Mock<ISeatService> _seatService;

        [TestMethod]
        public void SeatsController_GetAsync_ReturnOk()
        {
            //Arrange
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
            SeatDomainModel seat = new SeatDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Number = 10,
                Row = 10
            };
            List<SeatDomainModel> _seats = new List<SeatDomainModel>();
            _seats.Add(seat);
            IEnumerable<SeatDomainModel> seats = _seats;
            Task<IEnumerable<SeatDomainModel>> responseTask = Task.FromResult(seats);
            _seatService = new Mock<ISeatService>();
            _seatService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            SeatsController _seatsController = new SeatsController(_seatService.Object);

            //Act
            var actionResult = _seatsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var result = (List<SeatDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(seat.Id, result[0].Id);
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
        }

        [TestMethod]
        public void SeatsController_GetAsyncReturnNull_ReturnNewList()
        {
            //Arrange
            int expectedResultCount = 0;
            int expectedStatusCode = 200;
            IEnumerable<SeatDomainModel> seats = null;
            Task<IEnumerable<SeatDomainModel>> responseTask = Task.FromResult(seats);
            _seatService = new Mock<ISeatService>();
            _seatService.Setup(x => x.GetAllAsync()).Returns(responseTask);
            SeatsController _seatsController = new SeatsController(_seatService.Object);

            //Act
            var actionResult = _seatsController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)actionResult).Value;
            var result = (List<SeatDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)actionResult).StatusCode);
            Assert.IsInstanceOfType(actionResult, typeof(OkObjectResult));
        }
    }
}

