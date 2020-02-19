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

namespace WinterWorkShop.Cinema.Tests.Controllers
{
    [TestClass]
    public class MoviesControllerTests
    {
        private Mock<IMovieService> _moviesService;

        [TestMethod]
        public void GetAsync_Return_All_Movies()
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
            _moviesService.Setup(x => x.GetAllMovies(true)).Returns(responseTask);
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
        public void GetAsync_Return_NewList()
        {
            //Arrange
            IEnumerable<MovieDomainModel> responseTask = null;
            int expectedResultCount = 0;
            int expectedStatusCode = 200;

            _moviesService = new Mock<IMovieService>();
            _moviesService.Setup(x => x.GetAllMovies(true)).Returns(responseTask);
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
        public void Post_Create_WhereIsMovieNull_ErrorResponse()
        {
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
    }
}
