using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IMovieService
    {
        /// <summary>
        /// Get all movies by current parameter
        /// </summary>
        /// <param name="isCurrent"></param>
        /// <returns></returns>
        IEnumerable<MovieDomainModel> GetAllMoviesAsync(bool? isCurrent);

        Task<IEnumerable<MovieDomainModel>> GetAllMoviesAsync();

        /// <summary>
        /// Get a movie by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MovieDomainModel> GetMovieByIdAsync(Guid id);

        /// <summary>
        /// Adds new movie to DB
        /// </summary>
        /// <param name="newMovie"></param>
        /// <returns></returns>
        Task<MovieDomainModel> AddMovie(MovieDomainModel newMovie);

        /// <summary>
        /// Update a movie to DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UpdateMovieResultModel> UpdateMovie(MovieDomainModel updateMovie);

        /// <summary>
        /// Delete a movie by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MovieDomainModel> DeleteMovie(Guid id);

        /// <summary>
        /// Gets top 10 movies by rating
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MovieDomainModel>> GetTopMoviesAsync();

        /// <summary>
        /// Get movies by given tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Task<IEnumerable<MovieDomainModel>> GetMoviesByTag(string tag);
    }
}
