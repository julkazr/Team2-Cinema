using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
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
        List<Seat> _seats;
        List<Guid> listSeatsId;

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
                Id = _reservation.seatId,
                Number = 10,
                Row = 10
            };
            _seats = new List<Seat>();
            _seats.Add(_seat);
            IEnumerable<Seat> seats = _seats;
            Task<IEnumerable<Seat>> responseSeats = Task.FromResult(seats);
            listSeatsId = new List<Guid>();
            listSeatsId.Add(_seat.Id);

            _reservationRepository = new Mock<IReservationRepository>();
            _seatsRepository = new Mock<ISeatsRepository>();
            _reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
            _reservationRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(_reservation));
            _reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>())).Returns(_reservation);
            _reservationRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(_reservation);
            _reservationRepository.Setup(x => x.Save());
            _seatsRepository.Setup(x => x.GetAll()).Returns(responseSeats);
            _seatsRepository.Setup(x => x.GetByGuid(It.IsAny<Guid>())).Returns(Task.FromResult(_seat));
            _seatsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_seat));
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
        public void ReservationService_AddReservation_When_Updating_Non_Existing_Movie()
        {
            //Arrange
            _reservationRepository = new Mock<IReservationRepository>();
            _reservationRepository.Setup(x => x.Insert(It.IsAny<Reservation>())).Throws(new DbUpdateException());
            _reservationRepository.Setup(x => x.Save());

            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.AddReservation(_reservationDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void ReservationService_DeleteReservation_ReturnDeletedReservation()
        {
            //arrange
            int id = 1;
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.DeleteReservation(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.id);
            Assert.IsInstanceOfType(result, typeof(ReservationDomainModel));
        }

        [TestMethod]
        public void ReservationService_DeleteReservationReturnNull_ReturnNull()
        {
            //Arrange
            int id = 1;
            _reservationRepository = new Mock<IReservationRepository>();
            _reservationRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns((Reservation)null);
            _reservationRepository.Setup(x => x.Save());
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.DeleteReservation(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void ReservationService_DeleteReservation_When_Deleted_Non_Existing_Movie()
        {
            //Arrange
            int id = 1;
            _reservationRepository = new Mock<IReservationRepository>();
            _reservationRepository.Setup(x => x.Delete(It.IsAny<int>())).Throws(new DbUpdateException());
            _reservationRepository.Setup(x => x.Save());
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.DeleteReservation(id).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void ReservationService_CheckReservation_HaveReservation_ReturnResult()
        {
            //Arrange
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);
            string expectedMessage = "Some of seats are already reserved";
            int expectedCount = 1;

            //Act
            var result = reservationService.CheckReservationForSeats(listSeatsId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.SeatsTaken.Count);
            Assert.AreEqual(expectedMessage, result.InfoMessage);
            Assert.IsFalse(result.SeatsAreFree);
            Assert.IsInstanceOfType(result, typeof(CheckReservationForSeatsDomainModel));
        }

        [TestMethod]
        public void ReservationService_CheckReservation_ReturnResult()
        {
            //Arrange
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);
            string expectedMessage = "Seats are free to reserve";
            int expectedCount = 0;
            listSeatsId = new List<Guid>();
            listSeatsId.Add(Guid.NewGuid());

            //Act
            var result = reservationService.CheckReservationForSeats(listSeatsId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.SeatsTaken.Count);
            Assert.AreEqual(expectedMessage, result.InfoMessage);
            Assert.IsTrue(result.SeatsAreFree);
            Assert.IsInstanceOfType(result, typeof(CheckReservationForSeatsDomainModel));
        }

        [TestMethod]
        public void ReservationService_CheckReservation_GetAllReturnNull_ReturnResult()
        {
            //Arrange
            _seatsRepository = new Mock<ISeatsRepository>();
            List<Reservation> list = new List<Reservation>();
            IEnumerable<Reservation> reservations = list;
            Task<IEnumerable<Reservation>> responseTask = Task.FromResult(reservations);
            _reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);
            string expectedMessage = "There are no reservations";
            int expectedCount = 0;
            listSeatsId = new List<Guid>();
            listSeatsId.Add(Guid.NewGuid());

            //Act
            var result = reservationService.CheckReservationForSeats(listSeatsId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.SeatsTaken.Count);
            Assert.AreEqual(expectedMessage, result.InfoMessage);
            Assert.IsTrue(result.SeatsAreFree);
            Assert.IsInstanceOfType(result, typeof(CheckReservationForSeatsDomainModel));
        }

        [TestMethod]
        public void ReservationService_CheckPosition_ReturnNull()
        {
            //Arrange
            listSeatsId = new List<Guid>();
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.CheckPositionBeforeReservation(listSeatsId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ReservationService_CheckPosition_WithOneSeat_ReturnResult()
        {
            //Arrange
            string expectedMessage = "You passed only one seet";
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.CheckPositionBeforeReservation(listSeatsId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.InfoMessage);
            Assert.IsTrue(result.CheckSucceed);
        }

        [TestMethod]
        public void ReservationService_CheckPosition_ReturnResult()
        {
            //Arrange
            _seatsRepository = new Mock<ISeatsRepository>();
            Seat seat = new Seat
            {
                AuditoriumId = 1,
                Id = Guid.NewGuid(),
                Number = 11,
                Row = 11
            };
            listSeatsId.Add(seat.Id);
            _seats.Add(seat);
            IEnumerable<Seat> seats = _seats;
            Task<IEnumerable<Seat>> responseSeats = Task.FromResult(seats);
            string expectedMessage = "Seets are not next to each other and they are not in same row";
            _seatsRepository.Setup(x => x.GetByGuid(It.IsAny<Guid>())).Returns(Task.FromResult(_seat));
            _seatsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_seat));
            _seatsRepository.Setup(x => x.GetAll()).Returns(responseSeats);
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);

            //Act
            var result = reservationService.CheckPositionBeforeReservation(listSeatsId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.InfoMessage);
            Assert.IsFalse(result.CheckSucceed);
        }

        [TestMethod]
        public void ReservationService_CheckReservationForProjection_GetAllReturnNull_ReturnResult()
        {
            //Arrange
            _seatsRepository = new Mock<ISeatsRepository>();
            List<Reservation> list = new List<Reservation>();
            IEnumerable<Reservation> reservations = list;
            Task<IEnumerable<Reservation>> responseTask = Task.FromResult(reservations);
            _reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);
            string expectedMessage = "There are no reservations";
            int expectedCount = 0;
            listSeatsId = new List<Guid>();
            listSeatsId.Add(Guid.NewGuid());
            Guid projectionId = Guid.NewGuid();

            //Act
            var result = reservationService.CheckReservationForSeatsForProjection(listSeatsId, projectionId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.SeatsTaken.Count);
            Assert.AreEqual(expectedMessage, result.InfoMessage);
            Assert.IsTrue(result.SeatsAreFree);
            Assert.IsInstanceOfType(result, typeof(CheckReservationForSeatsDomainModel));

        }

        [TestMethod]
        public void ReservationService_CheckReservationForProjection_ReturnResult()
        {
            //Arrange
            _seatsRepository = new Mock<ISeatsRepository>();
            Reservation reservation = new Reservation
            {
                id = 1,
                projectionId = Guid.NewGuid(),
                seatId = Guid.NewGuid()
            };
            List<Reservation> list = new List<Reservation>();
            list.Add(reservation);
            IEnumerable<Reservation> reservations = list;
            Task<IEnumerable<Reservation>> responseTask = Task.FromResult(reservations);
            _reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);
            string expectedMessage = "Seats are free to reserve";
            int expectedCount = 0;
            listSeatsId = new List<Guid>();
            listSeatsId.Add(Guid.NewGuid());
            Guid projectionId = reservation.projectionId;

            //Act
            var result = reservationService.CheckReservationForSeatsForProjection(listSeatsId, projectionId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.SeatsTaken.Count);
            Assert.AreEqual(expectedMessage, result.InfoMessage);
            Assert.IsTrue(result.SeatsAreFree);
            Assert.IsInstanceOfType(result, typeof(CheckReservationForSeatsDomainModel));
        }

        [TestMethod]
        public void ReservationService_CheckReservationForProjection_SeatsIsAlreadyForReserve_ReturnResult()
        {
            //Arrange
            _seatsRepository = new Mock<ISeatsRepository>();
            Reservation reservation = new Reservation
            {
                id = 1,
                projectionId = Guid.NewGuid(),
                seatId = Guid.NewGuid()
            };
            List<Reservation> list = new List<Reservation>();
            list.Add(reservation);
            IEnumerable<Reservation> reservations = list;
            Task<IEnumerable<Reservation>> responseTask = Task.FromResult(reservations);
            _reservationRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ReservationService reservationService = new ReservationService(_reservationRepository.Object, _seatsRepository.Object);
            string expectedMessage = "Some of seats are already reserved";
            int expectedCount = 1;
            listSeatsId = new List<Guid>();
            listSeatsId.Add(reservation.seatId);
            Guid projectionId = reservation.projectionId;

            //Act
            var result = reservationService.CheckReservationForSeatsForProjection(listSeatsId, projectionId).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.SeatsTaken.Count);
            Assert.AreEqual(expectedMessage, result.InfoMessage);
            Assert.IsFalse(result.SeatsAreFree);
            Assert.IsInstanceOfType(result, typeof(CheckReservationForSeatsDomainModel));
        }

    }
}
