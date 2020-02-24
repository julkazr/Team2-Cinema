using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDomainModel>> GetAllAsync();

        Task<ReservationDomainModel> GetByIdAsync(int id);

        Task<ReservationDomainModel> AddReservation(ReservationDomainModel newReservation);

        Task<ReservationDomainModel> UpdateReservation(ReservationDomainModel updateReservation);

        Task<ReservationDomainModel> DeleteReservation(int id);
    }
}
