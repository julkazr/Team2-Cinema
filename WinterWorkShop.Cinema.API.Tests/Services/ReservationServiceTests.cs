using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class ReservationServiceTests
    {
        private Mock<IReservationRepository> _reservationRepository;
        private Mock<ISeatsRepository> _seatsRepository;

        Reservation _reservation;
        ReservationDomainModel _reservationDomainModel;
        Seat _seat;

        [TestInitialize]
        public void TestInitialize()
        {
            _reservation = new Reservation
            {
                id = 1,
                projectionId = Guid.NewGuid(),
                seatId = Guid.NewGuid()
            };
            _reservationDomainModel = new ReservationDomainModel
            {
                id = _reservation.id,
                projectionId = _reservation.projectionId,
                seatId = _reservation.seatId
            };
            List<Reservation> _reservations = new List<Reservation>();
            _reservations.Add(_reservation);
            IEnumerable<Reservation> reservations = _reservations;
            Task<IEnumerable<Reservation>> responseTask = Task.FromResult(reservations);

            _seat = new Seat
            {
                AuditoriumId = 1,
                Id = Guid.NewGuid(),
                Number = 10,
                Row = 10
            };
            List<Seat> _seats = new List<Seat>();
            _seats.Add(_seat);
            IEnumerable<Seat> seats = _seats;
            Task<IEnumerable<Seat>> responseSeats = Task.FromResult(seats);

            _reservationRepository = new Mock<IReservationRepository>();
            _seatsRepository = new Mock<ISeatsRepository>();
            _reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
            _reservationRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(_reservation));
            _reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>())).Returns(_reservation);
            _reservationRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(_reservation);
            _reservationRepository.Setup(x => x.Save());
            _seatsRepository.Setup(x => x.GetAll()).Returns(responseSeats);
        }

        [TestMethod]
        public void ReservationService_GetAll_ReturnAllReservation()
        {
            //Arrange
            int expectedCount = 1;
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var actionResult = reservationService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<ReservationDomainModel>)actionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(_reservation.id, result[0].id);
            Assert.IsInstanceOfType(result[0], typeof(ReservationDomainModel));
        }


        [TestMethod]
        public void ReservationService_GetAllReturnNull_ReturnNull()
        {
            //Arrange
            IEnumerable<Reservation> reservations = null;
            Task<IEnumerable<Reservation>> responseTask = Task.FromResult(reservations);
            _reservationRepository = new Mock<IReservationRepository>();
            _reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var actionResult = reservationService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<ReservationDomainModel>)actionResult;

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ReservationService_GetById_ReturnReservationById()
        {
            //Arrange
            int id = 1;
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.GetByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.id);
            Assert.IsInstanceOfType(result, typeof(ReservationDomainModel));
        }

        [TestMethod]
        public void ReservationService_GetByIdReturnNull_ReturnNull()
        {
            //Arrange
            int id = 1;
            _reservation = null;
            _reservationRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(_reservation));
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.GetByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ReservationService_AddReservation_ReturnInsertedReservation()
        {
            //Arrange
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.AddReservation(_reservationDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_reservationDomainModel.id, result.id);
            Assert.IsInstanceOfType(result, typeof(ReservationDomainModel));

        }

        [TestMethod]
        public void ReservationService_AddReservationReturnNull_ReturnNull()
        {
            //Arrange
            _reservationRepository = new Mock<IReservationRepository>();
            _reservation = null;
            _reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>())).Returns(_reservation);
            _reservationRepository.Setup(x => x.Save());
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.AddReservation(_reservationDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void Reservationservice_AddReservation_When_Updating_Non_Existing_Movie()
        {
            //Arrange
            _reservationRepository = new Mock<IReservationRepository>();
            _reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>())).Throws(new DbUpdateException());
            _reservationRepository.Setup(x => x.Save());

            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.AddReservation(_reservationDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
