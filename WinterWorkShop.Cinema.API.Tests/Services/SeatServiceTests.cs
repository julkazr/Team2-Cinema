using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class SeatServiceTests
    { 
        private Mock<ISeatsRepository> _seatsRepository;
        private Seat _seat;
        private List<Seat> _seats;

        [TestInitialize]
        public void TestInitialize()
        {
            _seats = new List<Seat>();
            _seat = new Seat
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Number = 10,
                Row = 10
            };
            _seats.Add(_seat);
            IEnumerable<Seat> seats = _seats;
            Task<IEnumerable<Seat>> responseTask = Task.FromResult(seats);

            _seatsRepository = new Mock<ISeatsRepository>();
            _seatsRepository.Setup(x => x.GetAll()).Returns(responseTask);
        }

        [TestMethod]
        public void SeatService_GetAll_ReturnAllSeats()
        {
            //Arrange
            int expectedCount = 1;
            SeatService seatService = new SeatService(_seatsRepository.Object);

            //Act
            var actionResult = seatService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<SeatDomainModel>)actionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(_seats[0].Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(SeatDomainModel));
        }

        [TestMethod]
        public void SeatService_GetAllReturnNull_ReturnNull()
        {
            //Arrange
            IEnumerable<Seat> seats = null;
            Task<IEnumerable<Seat>> responseTask = Task.FromResult(seats);
            _seatsRepository = new Mock<ISeatsRepository>();
            _seatsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            SeatService seatService = new SeatService(_seatsRepository.Object);

            //Act
            var actionResult = seatService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<SeatDomainModel>)actionResult;

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SeatService_DeleteSeat_ReturnDeletedSeat()
        {
            //Arrange
            int id = 1;
            _seatsRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(_seat);
            SeatService seatService = new SeatService(_seatsRepository.Object);

            //Act
            var result = seatService.DeleteSeat(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_seat.Id, result.Id);
            Assert.IsInstanceOfType(result, typeof(SeatDomainModel));
        }

        [TestMethod]
        public void SeatService_DeleteSeatReturnNull_ReturnNull()
        {
            //Arrange
            int id = 1;
            _seatsRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns((Seat)null);
            SeatService seatService = new SeatService(_seatsRepository.Object);

            //Act
            var result = seatService.DeleteSeat(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }
    }
}
