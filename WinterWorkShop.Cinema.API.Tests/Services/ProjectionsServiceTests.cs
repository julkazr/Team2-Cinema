using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProjectionsServiceTests
    {
        private Mock<IProjectionsRepository> _mockProjectionsRepository;
        private Mock<IReservationRepository> _mockReservationRepository;
        private Projection _projection;
        private ProjectionDomainModel _projectionDomainModel;
        private Reservation _reservation;
        private FilterProjectionDomainModel _filterProjectionDomainModel;
        private ProjectionWithAuditoriumResultModel _projectionWithAuditoriumResultModel;
        private SeatDomainModel _seatDomainModel;

        [TestInitialize]
        public void TestInitialize()
        {
            Seat seat = new Seat
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                Number = 1,
                Row = 1
            };
            Reservation reservation = new Reservation
            {
                id = 1,
                Seat = seat,
                seatId = seat.Id
            };
            List<Reservation> reservations = new List<Reservation>();
            reservations.Add(reservation);

            _filterProjectionDomainModel = new FilterProjectionDomainModel
            {
                auditoriumId = 1,
                cinemaId = 1,
                fromTime = DateTime.Now,
                movieId = Guid.NewGuid(),
                toTime = DateTime.Now.AddDays(1)
            };

            _projection = new Projection
            {
                Id = Guid.NewGuid(),
                Auditorium = new Auditorium { Name = "ImeSale" },
                Movie = new Movie { Title = "ImeFilma" },
                MovieId = Guid.NewGuid(),
                DateTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1,
                Reservations = reservations
            };

            _reservation = new Reservation
            {
                id = 1,
                Projection = _projection,
                projectionId = _projection.Id,
                seatId = Guid.NewGuid()
            };

            _projectionDomainModel = new ProjectionDomainModel
            {
                Id = Guid.NewGuid(),
                AditoriumName = "ImeSale",
                AuditoriumId = 1,
                MovieId = Guid.NewGuid(),
                MovieTitle = "ImeFilma",
                ProjectionTime = DateTime.Now.AddDays(1)
            };

            _seatDomainModel = new SeatDomainModel
            {
                Id = Guid.NewGuid(),
                Row = 1,
                Number = 1,
                AuditoriumId = 1
            };

            List < SeatDomainModel > seats = new List<SeatDomainModel>();
            seats.Add(_seatDomainModel);

            _projectionWithAuditoriumResultModel = new ProjectionWithAuditoriumResultModel
            {
                Projection = new ProjectionDomainModel
                {
                    Id = Guid.NewGuid(),
                    AditoriumName = "auditName",
                    AuditoriumId = 1,

                    MovieId = Guid.NewGuid(),
                    MovieTitle = "Title",
                    ProjectionTime = DateTime.Now
                },
                Auditorium = new AuditoriumDomainModel
                {
                    CinemaId = 1,
                    SeatsList = seats
                },
                Movie = new MovieDomainModel
                {
                    Rating = 7,
                    Year = 2020
                }
            };

            List<Projection> projectionsModelsList = new List<Projection>();

            projectionsModelsList.Add(_projection);
            IEnumerable<Projection> projections = projectionsModelsList;
            Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            _mockProjectionsRepository.Setup(x => x.GetByIdWithReservationsAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_projection));
            _mockReservationRepository = new Mock<IReservationRepository>();
            _mockReservationRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns(_reservation);
        }

        [TestMethod]
        public void ProjectionService_GetAllAsync_ReturnListOfProjecrions()
        {
            //Arrange
            int expectedResultCount = 1;
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var resultAction = projectionsController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<ProjectionDomainModel>)resultAction;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResultCount, result.Count);
            Assert.AreEqual(_projection.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(ProjectionDomainModel));
        }

        [TestMethod]
        public void ProjectionService_GetAllAsync_ReturnNull()
        {
            //Arrange
            IEnumerable<Projection> projections = null;
            Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var resultAction = projectionsController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(resultAction);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return list with projections
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - true
        // return ErrorMessage
        [TestMethod]
        public void ProjectionService_CreateProjection_WithProjectionAtSameTime_ReturnErrorMessage()
        {
            //Arrange
            List<Projection> projectionsModelsList = new List<Projection>();
            projectionsModelsList.Add(_projection);
            string expectedMessage = "Cannot create new projection, there are projections at same time alredy.";

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projectionsModelsList);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return empty list
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - false
        // _projectionsRepository.Insert(newProjection) mocked to return null
        //  if (insertedProjection == null) - true
        // return CreateProjectionResultModel  with errorMessage
        [TestMethod]
        public void ProjectionService_CreateProjection_InsertMockedNull_ReturnErrorMessage()
        {
            //Arrange
            List<Projection> projectionsModelsList = new List<Projection>();
            _projection = null;
            string expectedMessage = "Error occured while creating new projection, please try again.";

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projectionsModelsList);
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Returns(_projection);
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(expectedMessage, resultAction.ErrorMessage);
            Assert.IsFalse(resultAction.IsSuccessful);
        }

        // _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId) mocked to return empty list
        // if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0) - false
        // _projectionsRepository.Insert(newProjection) mocked to return valid EntityEntry<Projection>
        //  if (insertedProjection == null) - false
        // return valid projection 
        [TestMethod]
        public void ProjectionService_CreateProjection_InsertMocked_ReturnProjection()
        {
            //Arrange
            List<Projection> projectionsModelsList = new List<Projection>();

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetByAuditoriumId(It.IsAny<int>())).Returns(projectionsModelsList);
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Returns(_projection);
            _mockProjectionsRepository.Setup(x => x.Save());
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(resultAction);
            Assert.AreEqual(_projection.Id, resultAction.Projection.Id);
            Assert.IsNull(resultAction.ErrorMessage);
            Assert.IsTrue(resultAction.IsSuccessful);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void Projectionervice_CreateProjection_When_Updating_Non_Existing_Movie()
        {
            // Arrange
            List<Projection> projectionsModelsList = new List<Projection>();

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.Insert(It.IsAny<Projection>())).Throws(new DbUpdateException());
            _mockProjectionsRepository.Setup(x => x.Save());
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var resultAction = projectionsController.CreateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void ProjectionService_FiletProjections_ReturnResult()
        {
            //Assert
            _projection = new Projection
            {
                DateTime = DateTime.Now,
                Auditorium = new Auditorium
                {
                    Id = (int)_filterProjectionDomainModel.auditoriumId,
                    CinemaId = 1
                },
                Id = Guid.NewGuid(),
                AuditoriumId = (int)_filterProjectionDomainModel.auditoriumId,
                Movie = new Movie
                {
                    Id = (Guid)_filterProjectionDomainModel.movieId
                },
                MovieId = (Guid)_filterProjectionDomainModel.movieId
            };
            List<Projection> projectionsModelsList = new List<Projection>();
            int expectedCount = 1;

            projectionsModelsList.Add(_projection);
            IEnumerable<Projection> projections = projectionsModelsList;
            Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.FilterProjections(_filterProjectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var domainmodel = (List<ProjectionDomainModel>)result;

            //Assert
            Assert.IsNotNull(domainmodel);
            Assert.AreEqual(expectedCount, domainmodel.Count());
            Assert.AreEqual(_projection.Id, domainmodel[0].Id);
            Assert.IsInstanceOfType(domainmodel[0], typeof(ProjectionDomainModel));
        }

        [TestMethod]
        public void ProjectionService_FiletProjections_ReturnNull()
        {
            //Assert
            int expectedCount = 0;
            IEnumerable<Projection> projections = null;
            Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.FilterProjections(_filterProjectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var domainmodel = (List<ProjectionDomainModel>)result;

            //Assert
            Assert.IsNull(domainmodel);
        }

        [TestMethod]
        public void ProjectionService_Delete_ReturnDeletedProjection()
        {
            //Arange
            Guid id = _projection.Id;
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();

            var responseTask = Task.FromResult(_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdWithReservationAsync(It.IsAny<Guid>())).Returns(responseTask);
            _mockProjectionsRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(_projection);

            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.DeleteProjection(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);            
        }

        [TestMethod]
        public void ProjectionService_DeleteReturnNull_ReturnNull()
        {
            //Arange
            Guid id = _projection.Id;
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            Projection projection = null;
            var responseTask = Task.FromResult(_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdWithReservationAsync(It.IsAny<Guid>())).Returns(responseTask);
            _mockProjectionsRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(projection);

            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.DeleteProjection(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ProjectionService_Delete_ReservationDeleteReturnNull_ReturnDeletedProjection()
        {
            //Arange
            Guid id = _projection.Id;
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockReservationRepository = new Mock<IReservationRepository>();

            var responseTask = Task.FromResult(_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdWithReservationAsync(It.IsAny<Guid>())).Returns(responseTask);
            _mockReservationRepository.Setup(x => x.Delete(It.IsAny<int>())).Returns((Reservation)null);

            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.DeleteProjection(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ProjectionService_Delete_GetReturnNull_ReturnNull()
        {
            //Arange
            Guid id = _projection.Id;
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            Projection projection = null;
            var responseTask = Task.FromResult(projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdWithReservationAsync(It.IsAny<Guid>())).Returns(responseTask);

            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.DeleteProjection(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void Projectionervice_DeleteProjection_When_Deleting_Non_Existing_Movie()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            var responseTask = Task.FromResult(_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdWithReservationAsync(It.IsAny<Guid>())).Returns(responseTask);
            _mockProjectionsRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Throws(new DbUpdateException());
            _mockProjectionsRepository.Setup(x => x.Save());
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var resultAction = projectionsController.DeleteProjection(id).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void ProjectionService_GetById_ReturnProjection()
        {
            //Arrange
            Guid id = _projection.Id;
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            var responseTask = Task.FromResult(_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdAsyncView(It.IsAny<Guid>())).Returns(responseTask);

            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.GetByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var domainModel = (ProjectionDomainModel)result;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(id, domainModel.Id);
        }

        [TestMethod]
        public void ProjectionService_GetByIdReturnNull_ReturnNull()
        {
            //Arrange
            Guid id = _projection.Id;
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            var responseTask = Task.FromResult((Projection)null);
            _mockProjectionsRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseTask);

            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.GetByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var domainModel = (ProjectionDomainModel)result;

            //Assert
            Assert.IsNull(domainModel);
        }

        [TestMethod]
        public void ProjectionService_Update_ReturnUpdatedProjection()
        {
            //Arange
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.Update(It.IsAny<Projection>())).Returns(_projection);
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.UpdateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_projection.Id, result.Id);
        }

        [TestMethod]
        public void ProjectionService_UpdateReturnNull_ReturnNull()
        {
            //Arange
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.Update(It.IsAny<Projection>())).Returns((Projection)null);
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.UpdateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void Projectionervice_UpdateProjection_When_Updating_Non_Existing_Movie()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            _mockProjectionsRepository.Setup(x => x.Update(It.IsAny<Projection>())).Throws(new DbUpdateException());
            _mockProjectionsRepository.Setup(x => x.Save());
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var resultAction = projectionsController.UpdateProjection(_projectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void ProjectionService_GetReserved_ReturnListReserved()
        {
            //Arrange
            Guid id = _projection.Id;
            int expectedCount = 1;
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var actionResult = projectionsController.GetReserverdSeetsForProjection(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<SeatDomainModel>)actionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.IsInstanceOfType(result[0], typeof(SeatDomainModel));
        }

        [TestMethod]
        public void ProjectionService_GetReservedReturnNull_ReturnNull()
        {
            //Arrange
            Guid id = _projection.Id;
            _mockProjectionsRepository.Setup(x => x.GetByIdWithReservationsAsync(It.IsAny<Guid>())).Returns(Task.FromResult((Projection)null));
            ProjectionService projectionsController = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var actionResult = projectionsController.GetReserverdSeetsForProjection(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(actionResult);
        }

        [TestMethod]
        public void ProjectionService_GetProjectionWithAuditorium_ReturnProjection()
        {
            //Arrange
            var seat = new Seat
            {
                Id = Guid.NewGuid(),
                Row = 1,
                Number = 1,
                AuditoriumId = 1
            };

            List<Seat> seats = new List<Seat>();
            seats.Add(seat);
            _projection = new Projection
            {
                Id = Guid.NewGuid(),
                AuditoriumId = _projectionWithAuditoriumResultModel.Auditorium.Id,
                DateTime = DateTime.Now,
                Auditorium = new Auditorium
                {
                    Id = _projectionWithAuditoriumResultModel.Auditorium.Id,
                    CinemaId = 1,
                    Name = "auditName",
                    Seats = seats
                },
                Movie = new Movie
                {
                    Year = _projectionWithAuditoriumResultModel.Movie.Year,
                    Rating = _projectionWithAuditoriumResultModel.Movie.Rating
                }
            };
            Guid id = _projection.Id;
            _mockProjectionsRepository = new Mock<IProjectionsRepository>();
            var responseTask = Task.FromResult(_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdWithAuditoriumIncluded(It.IsAny<Guid>())).Returns(responseTask);

            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.GetProjectionWithAuditorium(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var model = (ProjectionWithAuditoriumResultModel)result;

            //Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(id, model.Projection.Id);
            Assert.IsInstanceOfType(model, typeof(ProjectionWithAuditoriumResultModel));
        }

        [TestMethod]
        public void ProjectionService_GetProjectionWithAuditorium_ReturnNull()
        {
            //Arrange
            //_projection = null;
            //List<Seat> seats = null;
            Guid id = _projection.Id;
            //Task<Projection> responseTask = Task.FromResult(_projection);
            _mockProjectionsRepository.Setup(x => x.GetByIdWithAuditoriumIncluded(It.IsAny<Guid>())).Returns(Task.FromResult((Projection)null));
            ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            //Act
            var result = projectionService.GetProjectionWithAuditorium(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var model = (ProjectionWithAuditoriumResultModel)result;

            //Assert
            Assert.IsNull(model);

            ////Assert
            //int expectedCount = 0;
            //IEnumerable<Projection> projections = null;
            //Task<IEnumerable<Projection>> responseTask = Task.FromResult(projections);

            //_mockProjectionsRepository = new Mock<IProjectionsRepository>();
            //_mockProjectionsRepository.Setup(x => x.GetAll()).Returns(responseTask);

            //ProjectionService projectionService = new ProjectionService(_mockProjectionsRepository.Object, _mockReservationRepository.Object);

            ////Act
            //var result = projectionService.FilterProjections(_filterProjectionDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
            //var domainmodel = (List<ProjectionDomainModel>)result;

            ////Assert
            //Assert.IsNull(domainmodel);
        }
    }
}
