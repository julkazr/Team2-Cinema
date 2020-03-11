using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Tests.Services
{
    [TestClass]
    public class MovieServiceTests
    {
        private Mock<IMoviesRepository> _moviesRepository;
        private Mock<IProjectionsRepository> _projectionsRepository;
        private Mock<ITagRepository> _tagRepository;
        private Movie _movie;
        private List<Movie> _movies;
        IEnumerable<Movie> movies;
        Task<IEnumerable<Movie>> responseTask;
        MovieDomainModel _movieDomainModel;
        List<TagMovie> tagMovies;

        [TestInitialize]
        public void TestInitialize()
        {
            _movies = new List<Movie>();
            _movie = new Movie
            {
                Id = Guid.NewGuid(),
                Current = true,
                Rating = 1,
                Title = "Title",
                Year = 2020
            };
            _movies.Add(_movie);
            movies = _movies;
            responseTask = Task.FromResult(movies);

            Projection projection = new Projection
            {
                Id = Guid.NewGuid(),
                MovieId = _movie.Id
            };
            List<Projection> projections = new List<Projection>();
            projections.Add(projection);

            TagMovie tagMovie = new TagMovie
            {
                Id = 1,
                MovieId = _movie.Id,
                Movie = _movie,
                TagId = 1
            };
            tagMovies = new List<TagMovie>();
            tagMovies.Add(tagMovie);

            Tag tag = new Tag
            {
                Id = 1,
                TagContent = "tag",
                TagMovies = tagMovies
            };
            List<Tag> _tags = new List<Tag>();
            _tags.Add(tag);
            IEnumerable<Tag> tags = _tags;
            Task<IEnumerable<Tag>> responseTag = Task.FromResult(tags);

            _movieDomainModel = new MovieDomainModel
            {
                Id = _movie.Id,
                Current = _movie.Current,
                Rating = (double)_movie.Rating,
                Title = _movie.Title,
                Year = _movie.Year
            };

            _moviesRepository = new Mock<IMoviesRepository>();
            _projectionsRepository = new Mock<IProjectionsRepository>();
            _tagRepository = new Mock<ITagRepository>();
            _moviesRepository.Setup(x => x.GetCurrentMovies()).Returns(_movies);
            _projectionsRepository.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(projections);
            _tagRepository.Setup(x => x.GetAll()).Returns(responseTag);
        }

        //MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

        [TestMethod]
        public void MovieService_GetCurrent_ReturnCurrentMovies()
        {
            //Arrange
            int expectedCount = 1;
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var actionResult = movieService.GetAllMoviesAsync(true);
            var result = (List<MovieDomainModel>)actionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(_movie.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(MovieDomainModel));
        }

        [TestMethod]
        public void MovieService_GetCurrent_ReturnNull()
        {
            //Arrange
            _moviesRepository = new Mock<IMoviesRepository>();
            _moviesRepository.Setup(x => x.GetCurrentMovies()).Returns((List<Movie>)null);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var actionResult = movieService.GetAllMoviesAsync(true);
            var result = (List<Movie>)actionResult;

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void MovieService_GetAll_ReturnAll()
        {
            //Arrange
            int expectedCount = 1;
            _moviesRepository.Setup(x => x.GetAll()).Returns(responseTask);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);
            //Act
            var actionResult = movieService.GetAllMoviesAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)actionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(_movie.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(MovieDomainModel));
        }

        [TestMethod]
        public void MovieService_GetAll_ReturnNull()
        {
            //Arrange
            movies = null;
            responseTask = Task.FromResult(movies);
            _moviesRepository.Setup(x => x.GetAll()).Returns(responseTask);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);
            //Act
            var actionResult = movieService.GetAllMoviesAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)actionResult;

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void MovieService_GetById_ReturnMovieById()
        {
            //Arrange
            Guid id = _movie.Id;
            var responseMovie = Task.FromResult(_movie);
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseMovie);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var result = movieService.GetMovieByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
            Assert.IsInstanceOfType(result, typeof(MovieDomainModel));        
        }

        [TestMethod]
        public void MovieService_GetByIdReturnNull_ReturnNull()
        {
            //Arrange
            Guid id = _movie.Id;
            Movie movie = null;
            var responseMovie = Task.FromResult(movie);
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseMovie);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var result = movieService.GetMovieByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void MovieService_AddMovie_ReturnInsertedMovie()
        {
            //Arrange
            _moviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns(_movie);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var result = movieService.AddMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_movieDomainModel.Id, result.Id);
            Assert.IsInstanceOfType(result, typeof(MovieDomainModel));
        }

        [TestMethod]
        public void MovieService_AddMovieReturnNull_ReturnReturnNull()
        {
            //Arrange
            _moviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Returns((Movie)null);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var result = movieService.AddMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void MovieService_AddMovie_When_Inserting_Non_Existing_Movie()
        {
            // Arrange
            List<Projection> projectionsModelsList = new List<Projection>();

            _moviesRepository = new Mock<IMoviesRepository>();
            _moviesRepository.Setup(x => x.Insert(It.IsAny<Movie>())).Throws(new DbUpdateException());
            _moviesRepository.Setup(x => x.Save());
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var resultAction = movieService.AddMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void MovieService_UpdateMovie_ReturnUpdatedMovie()
        {
            //Arrange
            bool expectedIsSuccessful = true;
            _moviesRepository.Setup(x => x.Update(It.IsAny<Movie>())).Returns(_movie);
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_movie));
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var result = movieService.UpdateMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedIsSuccessful, result.IsSuccessful);
            Assert.IsNull(result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(UpdateMovieResultModel));
        }

        [TestMethod]
        public void MovieService_UpdateMovie_ReturnNull()
        {
            //Arrange
            Projection projection = new Projection
            {
                Id = Guid.NewGuid(),
                DateTime = DateTime.Now.AddDays(1)
            };
            List<Projection> projections = new List<Projection>();
            _movieDomainModel.Current = !_movie.Current;
            projections.Add(projection);
            string expectedMessage = Messages.PROJECTION_EXISTING_FOR_MOVIE_ERROR;
            bool expectedIsSuccessful = false;
            _projectionsRepository = new Mock<IProjectionsRepository>();
            _projectionsRepository.Setup(x => x.GetByMovieId(It.IsAny<Guid>())).Returns(projections);
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(_movie));
            _moviesRepository.Setup(x => x.Update(It.IsAny<Movie>())).Returns(_movie);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var result = movieService.UpdateMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedIsSuccessful, result.IsSuccessful);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(UpdateMovieResultModel));
        }

        [TestMethod]
        public void MovieService_UpdateMovieReturnNull_ReturnUNull()
        {
            //Arrange
            string expectedMessage = Messages.UPDATING_MOVIE_ERROR;
            bool expectedIsSuccessful = false;
            _moviesRepository.Setup(x => x.Update(It.IsAny<Movie>())).Returns((Movie)null);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var result = movieService.UpdateMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedIsSuccessful, result.IsSuccessful);
            Assert.AreEqual(expectedMessage, result.ErrorMessage);
            Assert.IsInstanceOfType(result, typeof(UpdateMovieResultModel));
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void MovieService_UpdateMovie_When_Updating_Non_Existing_Movie()
        {
            // Arrange
            List<Projection> projectionsModelsList = new List<Projection>();

            _moviesRepository = new Mock<IMoviesRepository>();
            _moviesRepository.Setup(x => x.Update(It.IsAny<Movie>())).Throws(new DbUpdateException());
            _moviesRepository.Setup(x => x.Save());
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var resultAction = movieService.UpdateMovie(_movieDomainModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void MovieService_DeleteMovie_ReturnDeletedMovie()
        {
            //Arrange
            Guid id = _movie.Id;
            _moviesRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(_movie);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var result = movieService.DeleteMovie(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
            Assert.IsInstanceOfType(result, typeof(MovieDomainModel));
        }

        [TestMethod]
        public void MovieService_DeleteMovie_ReturnNull()
        {
            //Arrange
            Guid id = _movie.Id;
            _moviesRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Returns((Movie)null);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var result = movieService.DeleteMovie(id).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert
            Assert.IsNull(result);
        }


        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void MovieService_DeleteMovie_When_Deleting_Non_Existing_Movie()
        {
            // Arrange
            List<Projection> projectionsModelsList = new List<Projection>();
            Guid id = Guid.NewGuid();
            _moviesRepository = new Mock<IMoviesRepository>();
            _moviesRepository.Setup(x => x.Delete(It.IsAny<Guid>())).Throws(new DbUpdateException());
            _moviesRepository.Setup(x => x.Save());
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var resultAction = movieService.DeleteMovie(id).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void MovieService_GetTopmovies_ReturnTopMovies()
        {
            //Arrange
            int expectedCount = 1;
            _moviesRepository.Setup(x => x.GetAll()).Returns(responseTask);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var actionResult = movieService.GetTopMoviesAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)actionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(_movie.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(MovieDomainModel));
        }

        [TestMethod]
        public void MovieService_GetTopmovies_ReturnNull()
        {
            //Arrange
            movies = null;
            responseTask = Task.FromResult(movies);
            _moviesRepository.Setup(x => x.GetAll()).Returns(responseTask);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var actionResult = movieService.GetTopMoviesAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)actionResult;

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void MovieService_GetTopmoviesWithYear_ReturnTopMovies()
        {
            //Arrange
            int expectedCount = 1;
            int year = 2020;
            _moviesRepository.Setup(x => x.GetAll()).Returns(responseTask);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var actionResult = movieService.GetTopMoviesAsync(year).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)actionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(_movie.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(MovieDomainModel));
        }

        [TestMethod]
        public void MovieService_GetTopmoviesWithYear_ReturnNull()
        {
            //Arrange
            int year = 2020;
            movies = null;
            responseTask = Task.FromResult(movies);
            _moviesRepository.Setup(x => x.GetAll()).Returns(responseTask);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var actionResult = movieService.GetTopMoviesAsync(year).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)actionResult;

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void MovieService_GetMovieWithProjections_ReturnMovies()
        {
            //Arrange
            int auditoriumId = 1;
            int expectedCount = 1;
            Projection projection = new Projection
            {
                Id = Guid.NewGuid(),
                MovieId = Guid.NewGuid(),
                DateTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            List<Projection> projections = new List<Projection>();
            projections.Add(projection);
            _movie = new Movie
            {
                Current = true,
                Id = projection.MovieId,
                Projections = projections,
                Rating = 1,
                Title = "title",
                Year = 2020
            };
            _movies = new List<Movie>();
            _movies.Add(_movie);
            IEnumerable<Movie> movies = _movies;
            Task<IEnumerable<Movie>> responseMovie = Task.FromResult(movies);
            _moviesRepository.Setup(x => x.GetMoviesWithTheirProjections()).Returns(responseMovie);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var actionResult = movieService.GetMoviesWithTheirProjectionsAsync(auditoriumId).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieProjectionsResultModel>)actionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.IsInstanceOfType(result[0], typeof(MovieProjectionsResultModel));
            Assert.IsTrue(result[0].IsSuccessful);
        }

        [TestMethod]
        public void MovieService_GetMovieWithProjections_ReturnNull()
        {
            //Arrange
            int auditoriumId = 1;
            Projection projection = new Projection
            {
                Id = Guid.NewGuid(),
                MovieId = Guid.NewGuid(),
                DateTime = DateTime.Now.AddDays(1),
                AuditoriumId = 1
            };
            List<Projection> projections = new List<Projection>();
            projections.Add(projection);
            _movie = new Movie
            {
                Current = true,
                Id = projection.MovieId,
                Projections = projections,
                Rating = 1,
                Title = "title",
                Year = 2020
            };
            _movies = new List<Movie>();
            _movies.Add(_movie);
            IEnumerable<Movie> movies = null;
            Task<IEnumerable<Movie>> responseMovie = Task.FromResult(movies);
            _moviesRepository.Setup(x => x.GetMoviesWithTheirProjections()).Returns(responseMovie);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var actionResult = movieService.GetMoviesWithTheirProjectionsAsync(auditoriumId).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieProjectionsResultModel>)actionResult;

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void MovieService_GetMovieBytag_ReturnMovies()
        {
            //Arrange
            int expectedCount = 1;
            string tag = "tag";
            _movie.TagMovies = tagMovies;
            var responseMovie = Task.FromResult(_movie);
            _moviesRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).Returns(responseMovie);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var actionResult = movieService.GetMoviesByTag(tag).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)actionResult;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedCount, result.Count);
            Assert.AreEqual(_movie.Id, result[0].Id);
            Assert.IsInstanceOfType(result[0], typeof(MovieDomainModel));
            Assert.IsTrue(result[0].Current);
        }

        [TestMethod]
        public void MovieService_GetMovieBytag_ReturnNull()
        {
            //Arrange
            string tag = "tag";
            _tagRepository = new Mock<ITagRepository>();
            IEnumerable<Tag> tags = null;
            Task<IEnumerable<Tag>> responseTag = Task.FromResult(tags);
            _tagRepository.Setup(x => x.GetAll()).Returns(responseTag);
            MovieService movieService = new MovieService(_moviesRepository.Object, _projectionsRepository.Object, _tagRepository.Object);

            //Act
            var actionResult = movieService.GetMoviesByTag(tag).ConfigureAwait(false).GetAwaiter().GetResult();
            var result = (List<MovieDomainModel>)actionResult;

            //Assert
            Assert.IsNull(result);
        }
    }
}
