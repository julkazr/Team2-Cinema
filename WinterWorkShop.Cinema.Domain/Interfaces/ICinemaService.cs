using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface ICinemaService
    {
        Task<IEnumerable<CinemaDomainModel>> GetAllAsync();

        Task<CinemaDomainModel> GetByIdAsync(int id);

        Task<CinemaDomainModel> AddCinema(CinemaDomainModel newCinema);

        Task<CinemaDomainModel> UpdateCinema(CinemaDomainModel updateCinema);

        Task<CinemaDomainModel> DeleteCinema(int id);
    }
}
