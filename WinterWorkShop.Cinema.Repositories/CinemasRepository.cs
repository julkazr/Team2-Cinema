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
    public interface ICinemasRepository : IRepository<Data.Cinema> {
        Data.Cinema DeleteCinemaComplete(int id);
    }

    public class CinemasRepository : ICinemasRepository
    {
        private CinemaContext _cinemaContext;

        public CinemasRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Data.Cinema Delete(object id)
        {
            Data.Cinema existing = _cinemaContext.Cinemas.Find(id);

            var result = _cinemaContext.Cinemas.Remove(existing);

            return result.Entity;
        }

        public async Task<IEnumerable<Data.Cinema>> GetAll()
        {
            var data = await _cinemaContext.Cinemas.ToListAsync();

            return data;
        }

        //DELETE CINEMA AND ALL DATA CONNECTED TO CINEMA - AUDITORIUM, SEATS, PROJECTIONS, RESERVATIONS
        public Data.Cinema DeleteCinemaComplete(int id)
        {
            var existing = _cinemaContext.Cinemas.Include(x => x.Auditoriums).ThenInclude(a => a.Seats).ThenInclude(s => s.Reservations)
                                                         .Include(x => x.Auditoriums).ThenInclude(a => a.Projections)
                                                         .Where(cinema => cinema.Id.Equals(id)).ToList().First();

            var result = _cinemaContext.Cinemas.Remove(existing);

            return result.Entity;
        }        

        public async Task<Data.Cinema> GetByIdAsync(object id)
        {
            var data = await _cinemaContext.Cinemas.FindAsync(id);

            return data;
        }

        public Data.Cinema Insert(Data.Cinema obj)
        {
            return _cinemaContext.Cinemas.Add(obj).Entity;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Data.Cinema Update(Data.Cinema obj)
        {
            var updatedEntry = _cinemaContext.Cinemas.Attach(obj);
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry.Entity;
        }
    }
}
