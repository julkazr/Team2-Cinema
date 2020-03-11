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
    public class MoviesControllerTests
    {
        private Mock<IMovieService> _moviesService;

        [TestMethod]
        public void GetAsyncCurrent_Return_Current_Movies()
        {
            //Arrange
            List<MovieDomainModel> movieDomainModelsList = new List<MovieDomainModel>();
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Id = Guid.NewGuid(),
                Current = true,
                Rating = 1,
                Title = "ImeFilma",
                Year = 2020
            };
            movieDomainModelsList.Add(movieDomainModel);

            IEnumerable<MovieDomainModel> responseTask = movieDomainModelsList;
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetAllMoviesAsync(true)).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.AreEqual(movieDomainModel.Id, movieDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAsyncCurrent_Return_NewList()
        {
            //Arrange
            IEnumerable<MovieDomainModel> responseTask = null;
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetAllMoviesAsync(true)).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAllAsync_Return_All_Movies()
        {
            //Arrange
            List<MovieDomainModel> movieDomainModelsList = new List<MovieDomainModel>();
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Id = Guid.NewGuid(),
                Current = true,
                Rating = 1,
                Title = "ImeFilma",
                Year = 2020
            };
            movieDomainModelsList.Add(movieDomainModel);

            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            int expectedResultCount = 1;
            int expectedStatusCode = 200;

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetAllMoviesAsync()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.AreEqual(movieDomainModel.Id, movieDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetById_Return_Movie()
        {
            //Arrange
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Id = Guid.NewGuid(),
                Current = true,
                Rating = 1,
                Title = "ImeFilma",
                Year = 2020
            };
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);
            int expectedStatusCode = 200;

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetAsync(movieDomainModel.Id).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (MovieDomainModel)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(movieDomainModel.Id, movieDomainModelResultList.Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetById_Return_NotFound()
        {
            //Arrange
            Task<MovieDomainModel> responseTask = Task.FromResult((MovieDomainModel)null);
            int expectedStatusCode = 404;
            var expectedMessage = Messages.MOVIE_DOES_NOT_EXIST;

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)result).Value;

            //Assert
            Assert.IsNotNull(resultList);
            Assert.AreEqual(expectedMessage, resultList);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetAllAsync_Return_NewList()
        {
            //Arrange
            List<MovieDomainModel> movieDomainModelsList = null;
            IEnumerable<MovieDomainModel> movieDomainModels = movieDomainModelsList;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomainModels);
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetAllMoviesAsync()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetAllAsync().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void Post_CreateMovie_IsCuccessful_TrueMovie()
        {
            //Arrange
            int expectedStatusCode = 201;

            MovieModel movieModel = new MovieModel
            {
                Current = true,
                Rating = 9,
                Title = "ImeFilma",
                Year = 2020
            };
            MovieDomainModel createMovieDomain = new MovieDomainModel
            {
                Id = Guid.NewGuid(),
                Current = movieModel.Current,
                Rating = movieModel.Rating,
                Title = movieModel.Title,
                Year = movieModel.Year
            };

            Task<MovieDomainModel> responseTask = Task.FromResult(createMovieDomain);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.Post(movieModel)
                                         .ConfigureAwait(false)
                                         .GetAwaiter()
                                         .GetResult();
            var createdResult = ((CreatedResult)result).Value;
            var movieDomainModel = (MovieDomainModel)createdResult;

            //Assert
            Assert.IsNotNull(movieDomainModel);
            Assert.AreEqual(movieDomainModel.Id, createMovieDomain.Id);
            Assert.IsInstanceOfType(result, typeof(CreatedResult));
            Assert.AreEqual(expectedStatusCode, ((CreatedResult)result).StatusCode);
        }

        [TestMethod]
        public void Post_Create_Throw_DbException_Movie()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;

            MovieModel movieModel = new MovieModel
            {
                Current = true,
                Rating = 9,
                Title = "ImeFilma",
                Year = 2020
            };
            MovieDomainModel createMovieDomain = new MovieDomainModel
            {
                Id = Guid.NewGuid(),
                Current = movieModel.Current,
                Rating = movieModel.Rating,
                Title = movieModel.Title,
                Year = movieModel.Year
            };

            Task<MovieDomainModel> responseTask = Task.FromResult(createMovieDomain);
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.Post(movieModel)
                             .ConfigureAwait(false)
                             .GetAwaiter()
                             .GetResult();
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
        public void Post_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            MovieModel movieModel = new MovieModel
            {
                Current = true,
                Rating = 9,
                Title = "ImeFilma",
                Year = 2020
            };

            _moviesService = new Mock<IMovieService>();
            MoviesController moviesController = new MoviesController(_moviesService.Object);
            moviesController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = moviesController.Post(movieModel)
                             .ConfigureAwait(false)
                             .GetAwaiter()
                             .GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Post_Create_WhereIsMovieNull_Return_InternalServerError()
        {
            //Arrange
            int expectedStatusCode = 500;
            ErrorResponseModel errorResponse = new ErrorResponseModel
            {
                ErrorMessage = Messages.MOVIE_CREATION_ERROR,
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
            MovieModel movieModel = new MovieModel
            {
                Current = true,
                Rating = 9,
                Title = "ImeFilma",
                Year = 2020
            };

            MovieDomainModel movieDomainModel = null;
            Task<MovieDomainModel> responseTask = Task.FromResult(movieDomainModel);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.AddMovie(It.IsAny<MovieDomainModel>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.Post(movieModel)
                                         .ConfigureAwait(false)
                                         .GetAwaiter()
                                         .GetResult();
            var resultResponse = (ObjectResult)result;
            ErrorResponseModel resultErrorResponse = (ErrorResponseModel)((ObjectResult)result).Value;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
            Assert.AreEqual(errorResponse.ErrorMessage, resultErrorResponse.ErrorMessage);
        }

        [TestMethod]
        public void GetMoviesWithTheirProjectionsAsync_Return_Ok()
        {
            //Arrange
            int auditoriumId = 1;
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
            Guid movieId = Guid.NewGuid();
            List<ProjectionDomainModel> projsList = new List<ProjectionDomainModel>();
            ProjectionDomainModel projMod = new ProjectionDomainModel
            {
                Id = Guid.NewGuid(),
                AuditoriumId = 1,
                MovieId = movieId,
                ProjectionTime = DateTime.Now.AddDays(1)
            };
            projsList.Add(projMod);
            List<MovieProjectionsResultModel> movieList = new List<MovieProjectionsResultModel>();
            MovieProjectionsResultModel movieProjectionsResultModel = new MovieProjectionsResultModel
            {
                Movie = new MovieDomainModel
                {
                    Id = movieId,
                    Title = "MovieTitle",
                    Year = 2020,
                    Rating = 10,
                    Current = true
                },
                Projections = projsList,
                ErrorMessage = null,
                IsSuccessful = true
            };
            movieList.Add(movieProjectionsResultModel);
            List<MovieProjectionsResultModel> resList = new List<MovieProjectionsResultModel>();
            foreach (var item in movieList)
            {
                    resList.Add(item);
            }
            IEnumerable<MovieProjectionsResultModel> movies = resList;
            var responseTask = Task.FromResult(movies);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetMoviesWithTheirProjectionsAsync(It.IsAny<int>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetMoviesWithTheirProjectionsAsync(auditoriumId).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = ((List<MovieProjectionsResultModel>)resultList);
            

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetMoviesWithTheirProjectionsAsync_ReturnNull_Return_BadRequest()
        {
            //Arrange
            int auditoriumId = 1;
            int expectedResultCount = 0;
            int expectedStatusCode = 400;
            IEnumerable<MovieProjectionsResultModel> movies = null;
            var responseTask = Task.FromResult(movies);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetMoviesWithTheirProjectionsAsync(It.IsAny<int>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetMoviesWithTheirProjectionsAsync(auditoriumId).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((BadRequestObjectResult)result).Value;
            var movieDomainModelResultList = ((List<MovieProjectionsResultModel>)resultList);

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, ((BadRequestObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void Put_UpdateMovie_Return_Accepted()
        {
            //Arrange
            int expectedStatusCode = 202;
            Guid id = Guid.NewGuid();
            UpdateMovieModel updateMovieModel = new UpdateMovieModel
            {
                Id = id,
                Current = true,
                Rating = 1,
                Title = "newTitle",
                Year = 2020
            };
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Id = id,
                Year = 2020,
                Title = "oldTitle",
                Rating = 1,
                Current = true
            };
            UpdateMovieResultModel updateMovieResultModel = new UpdateMovieResultModel
            {
                ErrorMessage = null,
                IsSuccessful = true,
                Movie = new MovieDomainModel
                {
                    Current = true,
                    Id = id,
                    Rating = 1,
                    Title = updateMovieModel.Title,
                    Year = 2020
                }
            };
            var resultMovie = Task.FromResult(movieDomainModel);
            var resultTask = Task.FromResult(updateMovieResultModel);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(resultMovie);
            _moviesService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Returns(resultTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.Put(id, updateMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((ObjectResult)result).Value;
            var domainModel = (UpdateMovieResultModel)objectResult;
            var resultResponse = (ObjectResult)result;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(updateMovieResultModel.IsSuccessful, domainModel.IsSuccessful);
            Assert.AreEqual(updateMovieResultModel.Movie.Id, domainModel.Movie.Id);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Put_With_UnValid_ModelState_Return_BadRequest()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            UpdateMovieModel updateMovieModel = null;
            string expectedMessage = "Invalid Model State";
            int expectedStatusCode = 400;

            _moviesService = new Mock<IMovieService>();
            MoviesController moviesController = new MoviesController(_moviesService.Object);
            moviesController.ModelState.AddModelError("key", "Invalid Model State");

            //Act
            var result = moviesController.Put(id, updateMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (BadRequestObjectResult)result;
            var createdResult = ((BadRequestObjectResult)result).Value;
            var errorResponse = ((SerializableError)createdResult).GetValueOrDefault("key");
            var message = (string[])errorResponse;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, message[0]);
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Put_GetByIdReturnNull_Return_BadRequest()
        {
            //Arrange
            int expectedStatusCode = 400;
            string expectedMessage = Messages.MOVIE_DOES_NOT_EXIST;
            Guid id = Guid.NewGuid();
            UpdateMovieModel updateMovieModel = new UpdateMovieModel
            {
                Current = true,
                Id = id,
                Rating = 1,
                Title = "title",
                Year = 2020
            };
            MovieDomainModel movieDomainModel = null;
            var responseTask = Task.FromResult(movieDomainModel);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.Put(id, updateMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (ObjectResult)result;
            var ObjectResult = ((ObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)ObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Put_Create_Throw_DbException_Movie()
        {
            //Arrange
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Guid id = Guid.NewGuid();
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);
            UpdateMovieModel updateMovieModel = new UpdateMovieModel
            {
                Current = true,
                Id = id,
                Rating = 1,
                Title = "title",
                Year = 2020
            };
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Id = id,
                Year = 2020,
                Title = "oldTitle",
                Rating = 1,
                Current = true
            };
            var resultMovie = Task.FromResult(movieDomainModel);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(resultMovie);
            _moviesService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.Put(id, updateMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void Put_IsSuccessfulIsFalse_Return_BadRequest()
        {
            //Arrange
            string expectedMessage = "Projections for movies exist, movie can not be deactivated.";
            int expectedStatusCode = 400;
            Guid id = Guid.NewGuid();
            UpdateMovieModel updateMovieModel = new UpdateMovieModel
            {
                Id = id,
                Current = true,
                Rating = 1,
                Title = "newTitle",
                Year = 2020
            };
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Id = id,
                Year = 2020,
                Title = "oldTitle",
                Rating = 1,
                Current = true
            };
            UpdateMovieResultModel updateMovieResultModel = new UpdateMovieResultModel
            {
                ErrorMessage = Messages.PROJECTION_EXISTING_FOR_MOVIE_ERROR,
                IsSuccessful = false
            };
            var resultMovie = Task.FromResult(movieDomainModel);
            var resultTask = Task.FromResult(updateMovieResultModel);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetMovieByIdAsync(It.IsAny<Guid>())).Returns(resultMovie);
            _moviesService.Setup(x => x.UpdateMovie(It.IsAny<MovieDomainModel>())).Returns(resultTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.Put(id, updateMovieModel).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((ObjectResult)result).Value;
            var domainModel = (ErrorResponseModel)objectResult;
            var resultResponse = (ObjectResult)result;
            var errorMessage = domainModel.ErrorMessage;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedMessage, errorMessage);
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);

        }

        [TestMethod]
        public void Delete_DeleteMovie_Return_Accepted()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            int expectedStatusCode = 202;
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Current = true,
                Id = id,
                Rating = 1,
                Title = "Title",
                Year = 2020
            };
            var responseTask = Task.FromResult(movieDomainModel);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var objectResult = ((ObjectResult)result).Value;
            var domainModel = (MovieDomainModel)objectResult;
            var resultResponse = (ObjectResult)result;

            //Assert
            Assert.IsNotNull(domainModel);
            Assert.AreEqual(movieDomainModel.Id, domainModel.Id);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void Delete_Delete_Throw_DbException_Movie()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            string expectedMessage = "Inner exception error message.";
            int expectedStatusCode = 400;
            Exception exception = new Exception("Inner exception error message.");
            DbUpdateException dbUpdateException = new DbUpdateException("Error.", exception);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Throws(dbUpdateException);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
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
        public void Delete_DeleteReturnNull_Return_InternalServerError()
        {
            //Arrange
            string expectedMessage = Messages.MOVIE_DOES_NOT_EXIST;
            int expectedStatusCode = 500;
            Guid id = Guid.NewGuid();
            MovieDomainModel movieDomainModel = null;
            var responseTask = Task.FromResult(movieDomainModel);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.DeleteMovie(It.IsAny<Guid>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.Delete(id).ConfigureAwait(false).GetAwaiter().GetResult();
            var resultResponse = (ObjectResult)result;
            var ObjectResult = ((ObjectResult)result).Value;
            var errorResult = (ErrorResponseModel)ObjectResult;

            //Assert
            Assert.IsNotNull(resultResponse);
            Assert.AreEqual(expectedMessage, errorResult.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, resultResponse.StatusCode);
        }

        [TestMethod]
        public void GetTops_Get_Return_Ok()
        {
            //Arrange
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Current = true,
                Id = Guid.NewGuid(),
                Rating = 1,
                Title = "Title",
                Year = 2020
            };
            List<MovieDomainModel> movieDomainModels = new List<MovieDomainModel>();
            movieDomainModels.Add(movieDomainModel);
            IEnumerable<MovieDomainModel> movieDomains = movieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomains);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetTopMoviesAsync()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetTops().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.AreEqual(movieDomainModel.Id, movieDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetTops_ReturnNull_Return_BadRequest()
        {
            //Arrange
            int expectedResultCount = 0;
            int expectedStatusCode = 400;
            List<MovieDomainModel> movieDomainModels = null;
            IEnumerable<MovieDomainModel> movieDomains = movieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomains);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetTopMoviesAsync()).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetTops().ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetByTak_GetMovies_Return_Ok()
        {
            //Arrange
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
            string tag = "abcd";
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Current = true,
                Id = Guid.NewGuid(),
                Rating = 1,
                Title = "Title",
                Year = 2020
            };
            List<MovieDomainModel> movieDomainModels = new List<MovieDomainModel>();
            movieDomainModels.Add(movieDomainModel);
            IEnumerable<MovieDomainModel> movieDomains = movieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomains);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetMoviesByTag(It.IsAny<string>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetByTAg(tag).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.AreEqual(movieDomainModel.Id, movieDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);

        }

        [TestMethod]
        public void GetByTak_ReturnNull_Return_BadRequest()
        {
            //Arrange
            int expectedResultCount = 0;
            int expectedStatusCode = 400;
            string tag = "abcd";
            List<MovieDomainModel> movieDomainModels = null;
            IEnumerable<MovieDomainModel> movieDomains = movieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomains);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetMoviesByTag(It.IsAny<string>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetByTAg(tag).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)result).StatusCode);

        }

        [TestMethod]
        public void GetTopsByYear_Get_Return_Ok()
        {
            //Arrange
            int expectedResultCount = 1;
            int expectedStatusCode = 200;
            MovieDomainModel movieDomainModel = new MovieDomainModel
            {
                Current = true,
                Id = Guid.NewGuid(),
                Rating = 1,
                Title = "Title",
                Year = 2020
            };
            List<MovieDomainModel> movieDomainModels = new List<MovieDomainModel>();
            movieDomainModels.Add(movieDomainModel);
            IEnumerable<MovieDomainModel> movieDomains = movieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomains);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetTopMoviesAsync(It.IsAny<int>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetTops(movieDomainModel.Year).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((OkObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.AreEqual(movieDomainModel.Id, movieDomainModelResultList[0].Id);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(expectedStatusCode, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public void GetTopsByYear_ReturnNull_Return_BadRequest()
        {
            //Arrange
            int expectedResultCount = 0;
            int expectedStatusCode = 400;
            List<MovieDomainModel> movieDomainModels = null;
            IEnumerable<MovieDomainModel> movieDomains = movieDomainModels;
            Task<IEnumerable<MovieDomainModel>> responseTask = Task.FromResult(movieDomains);

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetTopMoviesAsync(It.IsAny<int>())).Returns(responseTask);
            MoviesController moviesController = new MoviesController(_moviesService.Object);

            //Act
            var result = moviesController.GetTops(2020).ConfigureAwait(false).GetAwaiter().GetResult().Result;
            var resultList = ((ObjectResult)result).Value;
            var movieDomainModelResultList = (List<MovieDomainModel>)resultList;

            //Assert
            Assert.IsNotNull(movieDomainModelResultList);
            Assert.AreEqual(expectedResultCount, movieDomainModelResultList.Count);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(expectedStatusCode, ((ObjectResult)result).StatusCode);
        }
    }
}
