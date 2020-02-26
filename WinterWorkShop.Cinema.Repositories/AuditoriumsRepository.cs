using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IAuditoriumsRepository : IRepository<Auditorium> 
    {
        Task<IEnumerable<Auditorium>> GetByAuditName(string name, int id);
    }
    public class AuditoriumsRepository : IAuditoriumsRepository
    {
        private CinemaContext _cinemaContext;

        public AuditoriumsRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }


        public async Task<IEnumerable<Auditorium>> GetByAuditName(string name, int id)
        {
            var data = await _cinemaContext.Auditoriums.Where(x => x.Name.Equals(name) && x.CinemaId.Equals(id)).ToListAsync();

            return data;
        }

        public Auditorium Delete(object id)
        {
            Auditorium existing = _cinemaContext.Auditoriums.Find(id);
            var seats = _cinemaContext.Seats.Where(x => x.AuditoriumId.Equals((int)id)).ToList();
            var result = _cinemaContext.Auditoriums.Remove(existing);
            result.Entity.Seats = seats;

            return result.Entity;
        }

        public async Task<IEnumerable<Auditorium>> GetAll()
        {
            //includovan seats
            var data = await _cinemaContext.Auditoriums.Include(x=>x.Seats).ToListAsync();

            return data;
        }

        public async Task<Auditorium> GetByIdAsync(object id)
        {
            var seats = _cinemaContext.Seats.Where(x => x.AuditoriumId.Equals((int)id)).ToList();
            List<Reservation> reservations = new List<Reservation>();
            foreach(var item in seats)
            {
                var reservation = _cinemaContext.Reservations.Where(x => x.seatId.Equals(item.Id)).ToList();
                foreach(var res in reservation)
                {
                    reservations.Add(res);
                    item.Reservations.Add(res);
                }
            }
            var result = await _cinemaContext.Auditoriums.FindAsync(id);
            result.Seats = seats;
            return result;
        }

        public Auditorium Insert(Auditorium obj)
        {
            var data = _cinemaContext.Auditoriums.Add(obj).Entity;

            return data;
        }

        public void Save()
        {
            _cinemaContext.SaveChanges();
        }

        public Auditorium Update(Auditorium obj)
        {
            var updatedEntry = _cinemaContext.Auditoriums.Attach(obj);
            _cinemaContext.Entry(obj).State = EntityState.Modified;

            return updatedEntry.Entity;
        }
    }
}
