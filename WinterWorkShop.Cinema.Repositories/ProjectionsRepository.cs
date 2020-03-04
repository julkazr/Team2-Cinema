using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }

    public class ProjectionsRepository : IProjectionsRepository
    {
        private CinemaContext _cinemaContext;

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

        public async Task<Projection> GetByIdWithReservationsAsync(object id)
        {
            var projections = await _cinemaContext.Projections.FindAsync(id);
            var reservations = _cinemaContext.Reservations.Include(x => x.Seat).Where(x => x.projectionId.Equals(projections.Id)).ToList();
            //var reservations = _cinemaContext.Reservations.Include(x => x.Seat).Where(x => x.projectionId.Equals(id)).ToList();
            //var auditoriums = _cinemaContext.Auditoriums.Where(x => x.Id.Equals(projections.AuditoriumId)).ToList();
            //projections.Auditorium = auditoriums[0];
            //projections.Movie = movies[0];
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
