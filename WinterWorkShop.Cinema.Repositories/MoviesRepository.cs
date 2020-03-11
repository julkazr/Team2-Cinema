using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IMoviesRepository : IRepository<Movie>
    {
        IEnumerable<Movie> GetCurrentMovies();
        Task<IEnumerable<Movie>> GetMoviesWithTheirProjections();
    }

    public class MoviesRepository : IMoviesRepository
    {
        private CinemaContext _cinemaContext;

        public MoviesRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Movie Delete(object id)
        {
            Movie existing = _cinemaContext.Movies.Find(id);

            if (existing == null)
            {
                return null;
            }

            var result = _cinemaContext.Movies.Remove(existing);

            return result.Entity;
        }

        public async Task<IEnumerable<Movie>> GetAll()
        {
            return await _cinemaContext.Movies.ToListAsync();
        }

        public async Task<Movie> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Movies.FindAsync(id);

            return data;
        }

        public IEnumerable<Movie> GetCurrentMovies()
        {
            var data = _cinemaContext.Movies
                .Where(x => x.Current);

            return data;
        }

        public Movie Insert(Movie obj)
        {
            var data = _cinemaContext.Movies.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Movie Update(Movie obj)
        {
            var updatedEntry = _cinemaContext.Movies.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }

        public async Task<IEnumerable<Movie>> GetMoviesWithTheirProjections()
        {
            var data = await _cinemaContext.Movies
                                           .Include(x => x.Projections)
                                           .ToListAsync();

            return data;
        }
    }
}
