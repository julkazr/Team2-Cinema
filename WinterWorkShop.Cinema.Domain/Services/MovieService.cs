using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMoviesRepository _moviesRepository;
        private readonly IProjectionsRepository _projectionsRepository;
        private readonly ITagRepository _tagRepository;

        public MovieService(IMoviesRepository moviesRepository, IProjectionsRepository projectionsRepository, ITagRepository tagRepository)
        {
            _moviesRepository = moviesRepository;
            _projectionsRepository = projectionsRepository;
            _tagRepository = tagRepository;
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

        public async Task<IEnumerable<MovieDomainModel>> GetAllMoviesAsync()
        {
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
                Rating = newMovie.Rating,
                Oscar = newMovie.Oscar
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
                Rating = data.Rating ?? 0,
                Oscar = data.Oscar ?? false
            };

            return domainModel;
        }

        public async Task<UpdateMovieResultModel> UpdateMovie(MovieDomainModel updateMovie)
        {

            var movieBeforeUpdate = await _moviesRepository.GetByIdAsync(updateMovie.Id);
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
                Rating = updateMovie.Rating,
                Oscar = updateMovie.Oscar
            };

            if (activeMovieProjections.Count == 0 || movieBeforeUpdate.Current == updateMovie.Current)
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
                    Rating = data.Rating ?? 0,
                    Oscar = data.Oscar ?? false
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

        public async Task<IEnumerable<MovieDomainModel>> GetTopMoviesAsync(int year)
        {
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
                    Year = item.Year,
                    Oscar = item.Oscar ?? false
                };
                result.Add(model);
            }

            List<MovieDomainModel> topTenResults = result.Where(x => x.Year.Equals(year)).OrderByDescending(x => x.Rating).Take(11).ToList();

            int len = topTenResults.Count;

            for (int i = 0; i < len; i++)
            {
                for (int j = i + 1; j < len; j++)
                {
                    if (topTenResults[i].Rating == topTenResults[j].Rating && topTenResults[j].Oscar && !topTenResults[i].Oscar)
                    {
                        var s = topTenResults[i];
                        topTenResults[i] = topTenResults[j];
                        topTenResults[j] = s;
                    }
                }
            }

            List<MovieDomainModel> resultTop = new List<MovieDomainModel>();

            resultTop = topTenResults.Take(10).ToList();

            return resultTop;

        }

        public async Task<IEnumerable<MovieProjectionsResultModel>> GetMoviesWithTheirProjectionsAsync(int auditoriumId)
        {
            var data = await _moviesRepository.GetMoviesWithTheirProjections();

            if (data == null)
            {
                return null;
            }

            List<MovieProjectionsResultModel> result = new List<MovieProjectionsResultModel>();
            MovieProjectionsResultModel model;

            foreach (var item in data)
            {
                List<Projection> listOfProjectionsForWantedAuditorium = item.Projections.Where(p => (p.AuditoriumId == auditoriumId) && (p.MovieId == item.Id)).ToList();

                List<ProjectionDomainModel> projsList = new List<ProjectionDomainModel>();

                foreach (var proj in listOfProjectionsForWantedAuditorium)
                {

                    if (proj.DateTime >= DateTime.Now)
                    {
                        ProjectionDomainModel projMod = new ProjectionDomainModel
                        {
                            Id = proj.Id,
                            //AditoriumName = proj.Auditorium.Name,
                            AuditoriumId = proj.AuditoriumId,
                            //MovieTitle = proj.Movie.Title,
                            MovieId = proj.MovieId,
                            ProjectionTime = proj.DateTime
                        };

                        projsList.Add(projMod);
                    }
                }

                model = new MovieProjectionsResultModel
                {
                    Movie = new MovieDomainModel
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Year = item.Year,
                        Rating = item.Rating ?? 0,
                        Current = item.Current
                    },
                    Projections = projsList,
                    IsSuccessful = true,
                    ErrorMessage = null
                };

                result.Add(model);
            }

            List<MovieProjectionsResultModel> resList = new List<MovieProjectionsResultModel>();

            foreach (var item in result)
            {
                if (item.Projections.Count() != 0)
                {
                    resList.Add(item);
                }
            }


            return resList;
        }

        //All movies by given tag
        public async Task<IEnumerable<MovieDomainModel>> GetMoviesByTag(string tag)
        {
            //Svi tagovi
            var allTags = await _tagRepository.GetAll();
            if (allTags == null)
            {
                return null;
            }

            //Lista pogodjenih tagova
            var listWithMatchedTag = allTags.Where(x => x.TagContent.Contains(tag, StringComparison.CurrentCultureIgnoreCase)).ToList();

            //lista svih filmova po serchovanom tagu
            List<Movie> listOfMovies = new List<Movie>();

            //Popunjavanje liste filmova na osnovu pogodjenog taga
            foreach (var matchedTag in listWithMatchedTag)
            {
                foreach (var item in matchedTag.TagMovies)
                {
                    Movie movieToAdd = await _moviesRepository.GetByIdAsync(item.MovieId);
                    listOfMovies.Add(movieToAdd);
                }
            }

            //Lista MovieDomainModel-a koja ce biti vracena
            List<MovieDomainModel> result = new List<MovieDomainModel>();
            MovieDomainModel model;
            foreach (var item in listOfMovies)
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


    }
}
