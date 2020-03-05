using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class CinemaServiceTests
    {
        private Mock<ICinemasRepository> _mockCinemaRepository;
        private CinemaDomainModel _cinemaDomainModel;
        private List<CinemaDomainModel> _cinemaDomainModels;
        private Data.Cinema _cinema;
        private List<Data.Cinema> _cinemas;

        [TestInitialize]
        public void TestInitialize()
        {
            _cinemas = new List<Data.Cinema>();
            _cinema = new Data.Cinema
            {
                Id = 1,
                Name = "nameOfCinema"
            };
            _cinemaDomainModels = new List<CinemaDomainModel>();
            _cinemaDomainModel = new CinemaDomainModel
            {
                Id = 1,
                Name = "nameOfCinema"
            };
            _cinemaDomainModels.Add(_cinemaDomainModel);
            _cinemas.Add(_cinema);

            IEnumerable<Data.Cinema> cinemas = _cinemas;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);

            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.GetAll()).Returns(responseTask);
        }

        [TestMethod]
        public void CinemaService_GetAll_ReturnAllCinemas()
        {
            //Arrange
            int expectedCount = 1;
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var resultAction = cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<CinemaDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(_cinema.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemaService_GetAll_ReturnNull()
        {
            //Arrange
            IEnumerable<Data.Cinema> cinemas = null;
            Task<IEnumerable<Data.Cinema>> responseTask = Task.FromResult(cinemas);
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.GetAll()).Returns(responseTask);
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var resultAction = cinemaService.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<CinemaDomainModel>)resultAction;

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CinemaService_GetById_ReturnCinema()
        {
            //Arrange
            int id = _cinema.Id;
            var responseTask = Task.FromResult(_cinema);
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var result = cinemaService.GetByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_cinema.Id, result.Id);
            Assert.IsInstanceOfType(result, typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemaService_GetById_ReturnNull()
        {
            //Arrange
            int id = _cinema.Id;
            var responseTask = Task.FromResult((Data.Cinema)null);
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(responseTask);
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var result = cinemaService.GetByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CinemaService_AddCinema_ReturnInsertedCinema()
        {
            //Arrange
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns(_cinema);
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var result = cinemaService.AddCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_cinemaDomainModel.Id, result.Id);
            Assert.IsInstanceOfType(result, typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemaService_AddCinema_ReturnNull()
        {
            //Arrange
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Returns((Data.Cinema)null);
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var result = cinemaService.AddCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void CinemaService_AddCinema_When_Inserting_Non_Existing_Cinema()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.Insert(It.IsAny<Data.Cinema>())).Throws(new DbUpdateException());
            _mockCinemaRepository.Setup(x => x.Save());
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var resultAction = cinemaService.AddCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void CinemaService_UpdateCinema_ReturnUpdatedCinema()
        {
            //Arrange
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.Update(It.IsAny<Data.Cinema>())).Returns(_cinema);
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var result = cinemaService.UpdateCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_cinemaDomainModel.Id, result.Id);
            Assert.IsInstanceOfType(result, typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemaService_UpdateCinema_ReturnNull()
        {
            //Arrange
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.Update(It.IsAny<Data.Cinema>())).Returns((Data.Cinema)null);
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var result = cinemaService.UpdateCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void CinemaService_UpdateCinema_When_Updating_Non_Existing_Cinema()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.Update(It.IsAny<Data.Cinema>())).Throws(new DbUpdateException());
            _mockCinemaRepository.Setup(x => x.Save());
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var resultAction = cinemaService.UpdateCinema(_cinemaDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void CinemaService_DeleteCinema_ReturnDeletedCinema()
        {
            //Arrange
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.DeleteCinemaComplete(It.IsAny<int>())).Returns(_cinema);
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var result = cinemaService.DeleteCinema(_cinema.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_cinemaDomainModel.Id, result.Id);
            Assert.IsInstanceOfType(result, typeof(CinemaDomainModel));
        }

        [TestMethod]
        public void CinemaService_DeleteCinema_ReturnNull()
        {
            //Arrange
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.DeleteCinemaComplete(It.IsAny<int>())).Returns((Data.Cinema)null);
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var result = cinemaService.DeleteCinema(_cinema.Id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void CinemaService_DeleteCinema_When_Deleting_Non_Existing_Cinema()
        {
            // Arrange
            int id = 1;
            _mockCinemaRepository = new Mock<ICinemasRepository>();
            _mockCinemaRepository.Setup(x => x.DeleteCinemaComplete(It.IsAny<int>())).Throws(new DbUpdateException());
            _mockCinemaRepository.Setup(x => x.Save());
            CinemaService cinemaService = new CinemaService(_mockCinemaRepository.Object);

            //Act
            var resultAction = cinemaService.DeleteCinema(id).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
