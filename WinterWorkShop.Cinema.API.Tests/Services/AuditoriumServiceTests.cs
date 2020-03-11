using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class AuditoriumServiceTests
    {
        private Mock<IAuditoriumsRepository> _auditoriumsRepository;
        private Mock<ICinemasRepository> _cinemasRepository;
        private Mock<ISeatsRepository> _seatsRepository;

        Auditorium auditorium;
        List<Auditorium> auditoriums;
        AuditoriumDomainModel auditoriumDomainModel;
        Seat seat;
        List<Seat> seats;
        Data.Cinema cinema;

        AuditoriumService auditoriumService;

        [TestInitialize]
        public void TestInitialize()
        {
            cinema = new Data.Cinema
            {
                Id = 1,
                Name = "name"
            };
            seat = new Seat
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Number = 1,
                Row = 1,
            };
            seats = new List<Seat>();
            seats.Add(seat);
            auditorium = new Auditorium
            {
                CinemaId = 1,
                Id = 1,
                Name = "auditName",
                Seats = seats
            };
            auditoriums = new List<Auditorium>();
            auditoriums.Add(auditorium);
            SeatDomainModel seatDomainModel = new SeatDomainModel
            {
                Id = seat.Id,
                AuditoriumId = seat.AuditoriumId,
                Number = 1,
                Row = 1
            };
            auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = 1,
                Id = 1,
                Name = auditorium.Name,
                SeatsList = new List<SeatDomainModel>()
            };
            auditoriumDomainModel.SeatsList.Add(seatDomainModel);

            IEnumerable<Auditorium> domainModels = auditoriums;
            IEnumerable<Auditorium> responseAuditorium = new List<Auditorium>();
            Task<IEnumerable<Auditorium>> responseTask = Task.FromResult(domainModels);

            _auditoriumsRepository = new Mock<IAuditoriumsRepository>();
            _cinemasRepository = new Mock<ICinemasRepository>();
            _seatsRepository = new Mock<ISeatsRepository>();
            _auditoriumsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            _auditoriumsRepository.Setup(x => x.Save());
            _auditoriumsRepository.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(responseAuditorium));
            _auditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns(auditorium);
            _auditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(auditorium));
            _auditoriumsRepository.Setup(x => x.Update(It.IsAny<Auditorium>())).Returns(auditorium);
            _auditoriumsRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(auditorium);
            _cinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(cinema));
            _seatsRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(seat);
            _seatsRepository.Setup(x => x.Insert(It.IsAny<Seat>())).Returns(seat);
            _seatsRepository.Setup(x => x.Save());

            auditoriumService = new AuditoriumService(_auditoriumsRepository.Object, _cinemasRepository.Object, _seatsRepository.Object);
        }

        [TestMethod]
        public void auditoriumService_GetAll_ReturnAllAuditoriums()
        {
            //Arrange
            int expectedCount = 1;

            //Act
            var actionResult = auditoriumService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<AuditoriumDomainModel>)actionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(auditorium.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(AuditoriumDomainModel));
        }

        [TestMethod]
        public void auditoriumService_GetAllReturnNull_ReturnNull()
        {
            //Arrange
            _auditoriumsRepository.Setup(x => x.GetAll()).Returns(Task.FromResult((IEnumerable<Auditorium>)null));

            //Act
            var actionResult = auditoriumService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(actionResult);
        }

        [TestMethod]
        public void auditoriumService_Create_ReturnCreatedAuditorium()
        {
            //Arrange
            auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = auditorium.CinemaId,
                Id = auditorium.Id,
                Name = auditorium.Name,
                SeatsList = new List<SeatDomainModel>()
            };
            SeatDomainModel seatDomain = new SeatDomainModel
            {
                AuditoriumId = auditorium.Id,
                Id = seat.Id,
                Number = 1,
                Row = 1
            };
            auditoriumDomainModel.SeatsList.Add(seatDomain);

            //Act
            var result = auditoriumService.CreateAuditorium(auditoriumDomainModel, 1, 1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ErrorMessage);
            Assert.AreEqual(auditorium.Id, result.Auditorium.Id);
            Assert.IsTrue(result.IsSuccessful);
            Assert.IsInstanceOfType(result, typeof(CreateAuditoriumResultModel));
        }

        [TestMethod]
        public void auditoriumService_Create_InsertReturnNull_ReturnCreateError()
        {
            //Arrange
            string expectedMessage = Messages.AUDITORIUM_CREATION_ERROR;
            auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = auditorium.CinemaId,
                Id = auditorium.Id,
                Name = auditorium.Name,
                SeatsList = new List<SeatDomainModel>()
            };
            SeatDomainModel seatDomain = new SeatDomainModel
            {
                AuditoriumId = auditorium.Id,
                Id = seat.Id,
                Number = 1,
                Row = 1
            };
            auditoriumDomainModel.SeatsList.Add(seatDomain);
            _auditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Returns((Auditorium)null);

            //Act
            var result = auditoriumService.CreateAuditorium(auditoriumDomainModel, 1, 1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsInstanceOfType(result, typeof(CreateAuditoriumResultModel));
        }

        [TestMethod]
        public void auditoriumService_Create_GetByAuditNameReturnAuditorium_ReturnError()
        {
            //Arrange
            string expectedMessage = Messages.AUDITORIUM_SAME_NAME;
            auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = auditorium.CinemaId,
                Id = auditorium.Id,
                Name = auditorium.Name,
                SeatsList = new List<SeatDomainModel>()
            };
            SeatDomainModel seatDomain = new SeatDomainModel
            {
                AuditoriumId = auditorium.Id,
                Id = seat.Id,
                Number = 1,
                Row = 1
            };
            auditoriumDomainModel.SeatsList.Add(seatDomain);
            _auditoriumsRepository.Setup(x => x.GetByAuditName(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult((IEnumerable<Auditorium>)auditoriums));

            //Act
            var result = auditoriumService.CreateAuditorium(auditoriumDomainModel, 1, 1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsInstanceOfType(result, typeof(CreateAuditoriumResultModel));
        }

        [TestMethod]
        public void auditoriumService_Create_GetByIdReturnNull_ReturnCreatedAuditorium()
        {
            //Arrange
            string expectedMessage = Messages.AUDITORIUM_UNVALID_CINEMAID;
            auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = auditorium.CinemaId,
                Id = auditorium.Id,
                Name = auditorium.Name,
                SeatsList = new List<SeatDomainModel>()
            };
            SeatDomainModel seatDomain = new SeatDomainModel
            {
                AuditoriumId = auditorium.Id,
                Id = seat.Id,
                Number = 1,
                Row = 1
            };
            auditoriumDomainModel.SeatsList.Add(seatDomain);
            _cinemasRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult((Data.Cinema)null));

            //Act
            var result = auditoriumService.CreateAuditorium(auditoriumDomainModel, 1, 1).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsFalse(result.IsSuccessful);
            Assert.IsInstanceOfType(result, typeof(CreateAuditoriumResultModel));
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void auditoriumService_Create_When_Inserting_Non_Existing_Auditorium()
        {
            //Arrange
            auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = auditorium.CinemaId,
                Id = auditorium.Id,
                Name = auditorium.Name,
                SeatsList = new List<SeatDomainModel>()
            };
            SeatDomainModel seatDomain = new SeatDomainModel
            {
                AuditoriumId = auditorium.Id,
                Id = seat.Id,
                Number = 1,
                Row = 1
            };
            auditoriumDomainModel.SeatsList.Add(seatDomain);
            _auditoriumsRepository.Setup(x => x.Insert(It.IsAny<Auditorium>())).Throws(new DbUpdateException());

            //Act
            var result = auditoriumService.CreateAuditorium(auditoriumDomainModel, 1, 1).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void auditoriumService_GetById_ReturnAuditoriumbyId()
        {
            //Arrange
            int id = 1;

            //Act
            var result = auditoriumService.GetByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
            Assert.IsInstanceOfType(result, typeof(AuditoriumDomainModel));
        }

        [TestMethod]
        public void auditoriumService_GetByIdReturnNull_ReturnReturnNull()
        {
            //Arrange
            int id = 1;
            _auditoriumsRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult((Auditorium)null));

            //Act
            var result = auditoriumService.GetByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void auditoriumService_Update_ReturnUpdatedAuditorium()
        {
            //Act
            var result = auditoriumService.UpdateAuditorium(auditoriumDomainModel, 1, 1, true).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(auditorium.Id, result.Id);
            Assert.IsInstanceOfType(result, typeof(AuditoriumDomainModel));
        }

        [TestMethod]
        public void auditoriumService_UpdateReturnNull_ReturnReturnNull()
        {
            //Arrange
            _auditoriumsRepository.Setup(x => x.Update(It.IsAny<Auditorium>())).Returns((Auditorium)null);
            //Act
            var result = auditoriumService.UpdateAuditorium(auditoriumDomainModel, 1, 1, true).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void auditoriumService_Update_SeatsNotFree_ReturnUpdatedAuditorium()
        {
            //Act
            var result = auditoriumService.UpdateAuditorium(auditoriumDomainModel, 1, 0, false).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void auditoriumService_Update_When_Update_Non_Existing_Auditorium()
        {
            //Arrange
            auditoriumDomainModel = new AuditoriumDomainModel
            {
                CinemaId = auditorium.CinemaId,
                Id = auditorium.Id,
                Name = auditorium.Name,
                SeatsList = new List<SeatDomainModel>()
            };
            SeatDomainModel seatDomain = new SeatDomainModel
            {
                AuditoriumId = auditorium.Id,
                Id = seat.Id,
                Number = 1,
                Row = 1
            };
            auditoriumDomainModel.SeatsList.Add(seatDomain);
            _auditoriumsRepository.Setup(x => x.Update(It.IsAny<Auditorium>())).Throws(new DbUpdateException());

            //Act
            var result = auditoriumService.UpdateAuditorium(auditoriumDomainModel, 1, 1, true).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void auditoriumService_Delete_returnDeletedAuditorium()
        {
            //Arrange
            int id = 1;

            //Act
            var result = auditoriumService.DeleteAuditorium(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
            Assert.IsInstanceOfType(result, typeof(AuditoriumDomainModel));
        }

        [TestMethod]
        public void auditoriumService_DeleteReturnNull_returnNull()
        {
            //Arrange
            int id = 1;
            _auditoriumsRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns((Auditorium)null);

            //Act
            var result = auditoriumService.DeleteAuditorium(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void auditoriumService_Delete_When_Delete_Non_Existing_Auditorium()
        {
            //Arrange
            int id = 1;
            _auditoriumsRepository.Setup(x => x.Delete(It.IsAny<int>())).Throws(new DbUpdateException());

            //Act
            var result = auditoriumService.DeleteAuditorium(id).ConfigureAwait(false).GetAwaiter().GetResult();
        }

    }
}
