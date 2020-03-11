using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IProjectionsRepository : IRepository<Projection>
    {
        IEnumerable<Projection> GetByAuditoriumId(int salaId);
        IEnumerable<Projection> GetByMovieId(Guid movieId);
        Task<Projection> GetByIdWithReservationAsync(object id);
        Task<Projection> GetByIdWithReservationsAsync(object id);
        Task<Projection> GetByIdWithAuditoriumIncluded(Guid id);
        Task<Projection> GetByIdAsyncView(object id);
    }

    public class ProjectionsRepository : IProjectionsRepository
    {
        private readonly CinemaContext _cinemaContext;

        public ProjectionsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Projection Delete(object id)
        {
            Projection existing = _cinemaContext.Projections.Find(id);
            var result = _cinemaContext.Projections.Remove(existing).Entity;

            return result;
        }

        public async Task<IEnumerable<Projection>> GetAll()
        {
            var data = await _cinemaContext.Projections.Include(x => x.Movie).Include(x => x.Auditorium).ToListAsync();

            return data;
        }

        public async Task<Projection> GetByIdAsync(object id)
        {
            var projections = await _cinemaContext.Projections.FindAsync(id);
            var movies = _cinemaContext.Movies.Where(x => x.Id.Equals(projections.MovieId)).ToList();
            var auditoriums = _cinemaContext.Auditoriums.Where(x => x.Id.Equals(projections.AuditoriumId)).ToList();
            projections.Auditorium = auditoriums[0];
            projections.Movie = movies[0];

            return projections;
        }
        //Metod ispod ima istu funkciju kao metod iznad, samo sto koristi kreiran view (performance task)
        public async Task<Projection> GetByIdAsyncView(object id)
        {
            var projections = _cinemaContext.ProjectionsWithMovieAndAuditoriums.ToList();
            var filteredProjeciton = projections.Where(x => x.Id.Equals(id)).First();

            Auditorium auditorium = new Auditorium
            {
                Id = filteredProjeciton.AuditoriumId,
                Name = filteredProjeciton.AuditoriumName
            };

            Movie movie = new Movie
            {
                Id = filteredProjeciton.MovieId,
                Title = filteredProjeciton.MovieTitle,
                Year = filteredProjeciton.MovieYear,
                Rating = filteredProjeciton.MovieRating,
                Current = filteredProjeciton.MovieCurrent,
                Oscar = filteredProjeciton.MovieOscar
            };

            Projection projection = new Projection
            {
                Id = filteredProjeciton.Id,
                MovieId = filteredProjeciton.MovieId,
                AuditoriumId = filteredProjeciton.AuditoriumId,
                DateTime = filteredProjeciton.projectionDateTime,
                Movie = movie,
                Auditorium = auditorium
            };

            return projection;
        }

        public async Task<Projection> GetByIdWithReservationsAsync(object id)
        {
            var projections = await _cinemaContext.Projections.FindAsync(id);
            var reservations = _cinemaContext.Reservations.Include(x => x.Seat).Where(x => x.projectionId.Equals(projections.Id)).ToList();
            projections.Reservations = reservations;

            return projections;
        }

        public async Task<Projection> GetByIdWithReservationAsync(object id)
        {

            Guid enteredID = new Guid(id.ToString());

            return await _cinemaContext.Projections.Include(x => x.Reservations).SingleOrDefaultAsync(x => x.Id == enteredID);
        }

        public IEnumerable<Projection> GetByAuditoriumId(int auditoriumId)
        {
            var projectionsData = _cinemaContext.Projections.Where(x => x.AuditoriumId == auditoriumId);

            return projectionsData;
        }

        public Projection Insert(Projection obj)
        {
            var data = _cinemaContext.Projections.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Projection Update(Projection obj)
        {
            var updatedEntry = _cinemaContext.Projections.Attach(obj).Entity;
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry;
        }

        public IEnumerable<Projection> GetByMovieId(Guid movieId)
        {
            var projectionsData = _cinemaContext.Projections.Where(x => x.MovieId == movieId);

            return projectionsData;
        }

        public async Task<Projection> GetByIdWithAuditoriumIncluded(Guid id)
        {
            Guid enteredId = new Guid(id.ToString());
            var data = await _cinemaContext.Projections
                                           .Include(p => p.Auditorium)
                                           .Include(x => x.Movie)
                                           .Include(x => x.Auditorium.Seats)
                                           .SingleOrDefaultAsync(p => p.Id == id);
            return data;
        }
    }
}
