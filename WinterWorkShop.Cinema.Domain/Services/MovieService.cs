using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Repositories;
using System.Linq;


namespace WinterWorkShop.Cinema.Domain.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMoviesRepository _moviesRepository;
        private readonly IProjectionsRepository _projectionsRepository;

        public MovieService(IMoviesRepository moviesRepository, IProjectionsRepository projectionsRepository)
        {
            _moviesRepository = moviesRepository;
            _projectionsRepository = projectionsRepository;
        }

        public IEnumerable<MovieDomainModel> GetAllMoviesAsync(bool? isCurrent)
        {
            var data = _moviesRepository.GetCurrentMovies();

            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year
                };
                result.Add(model);
            }

            return result;

        }

        public async Task<MovieDomainModel> GetMovieByIdAsync(Guid id)
        {
            var data = await _moviesRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Current = data.Current,
                Rating = data.Rating ?? 0,
                Title = data.Title,
                Year = data.Year
            };

            return domainModel;
        }

        public async Task<MovieDomainModel> AddMovie(MovieDomainModel newMovie)
        {
            Movie movieToCreate = new Movie()
            {
                Title = newMovie.Title,
                Current = newMovie.Current,
                Year = newMovie.Year,
                Rating = newMovie.Rating
            };

            var data = _moviesRepository.Insert(movieToCreate);
            if (data == null)
            {
                return null;
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel()
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0
            };

            return domainModel;
        }

        public async Task<UpdateMovieResultModel> UpdateMovie(MovieDomainModel updateMovie) {

            var projectionsForMovie = _projectionsRepository.GetByMovieId(updateMovie.Id);
            List<Projection> activeMovieProjections = new List<Projection>();

            foreach (var item in projectionsForMovie)
            {
                if (item.DateTime >= DateTime.Now)
                {
                    activeMovieProjections.Add(item);
                }
            }

            Movie movie = new Movie
            {
                Id = updateMovie.Id,
                Title = updateMovie.Title,
                Year = updateMovie.Year,
                Rating = updateMovie.Rating
            };

            if (activeMovieProjections.Count == 0)
            {
                movie.Current = updateMovie.Current;
            }
            else
            {
                return new UpdateMovieResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_EXISTING_FOR_MOVIE_ERROR
                };

            }


            var data = _moviesRepository.Update(movie);

            if (data == null)
            {
                return new UpdateMovieResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.UPDATING_MOVIE_ERROR
                };
            }

            _moviesRepository.Save();

            UpdateMovieResultModel resultModel = new UpdateMovieResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Movie = new MovieDomainModel
                {
                    Id = data.Id,
                    Title = data.Title,
                    Current = data.Current,
                    Year = data.Year,
                    Rating = data.Rating ?? 0
                }
            };
            return resultModel;
        }

        public async Task<MovieDomainModel> DeleteMovie(Guid id)
        {
            var data = _moviesRepository.Delete(id);

            if (data == null)
            {
                return null;
            }

            _moviesRepository.Save();

            MovieDomainModel domainModel = new MovieDomainModel
            {
                Id = data.Id,
                Title = data.Title,
                Current = data.Current,
                Year = data.Year,
                Rating = data.Rating ?? 0

            };
            
            return domainModel;
        }
        //*************************************************************************************

        public async Task<IEnumerable<MovieDomainModel>> GetTopMoviesAsync()
        {
            //var data = _moviesRepository.GetCurrentMovies();
            var data = await _moviesRepository.GetAll();


            if (data == null)
            {
                return null;
            }

            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in data)
            {
                model = new MovieDomainModel
                {
                    Current = item.Current,
                    Id = item.Id,
                    Rating = item.Rating ?? 0,
                    Title = item.Title,
                    Year = item.Year
                };
                result.Add(model);
            }

            List<MovieDomainModel> topTenResults = result.OrderByDescending(x => x.Rating).Take(10).ToList();

            return topTenResults;

        }
    }
}
