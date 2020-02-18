using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class CinemaService : ICinemaService
    {
        private readonly ICinemasRepository _cinemasRepository;

        public CinemaService(ICinemasRepository cinemasRepository)
        {
            _cinemasRepository = cinemasRepository;
        }

        public async Task<IEnumerable<CinemaDomainModel>> GetAllAsync()
        {
            var data = await _cinemasRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<CinemaDomainModel> result = new List<CinemaDomainModel>();
            CinemaDomainModel model;
            foreach (var item in data)
            {
                model = new CinemaDomainModel()
                {
                    Id = item.Id,
                    Name = item.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CinemaDomainModel> GetByIdAsync(int id)
        {
            var data = await _cinemasRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            CinemaDomainModel domainModel = new CinemaDomainModel()
            {
                Id = data.Id,
                Name = data.Name
            };

            return domainModel;
        }

        public async Task<CinemaDomainModel> AddCinema(CinemaDomainModel newCinema)
        {
            Data.Cinema cinemaToCreate = new Data.Cinema()
            {
                Id = newCinema.Id,
                Name = newCinema.Name
            };

            var data = _cinemasRepository.Insert(cinemaToCreate);
            if(data == null)
            {
                return null;
            }

            _cinemasRepository.Save();

            CinemaDomainModel domainModel = new CinemaDomainModel()
            {
                Id = data.Id,
                Name = data.Name
            };

            return domainModel;
        }

        public async Task<CinemaDomainModel> UpdateCinema(CinemaDomainModel updateCinema)
        {
            Data.Cinema cinema = new Data.Cinema()
            {
                Id = updateCinema.Id,
                Name = updateCinema.Name
            };

            var data = _cinemasRepository.Update(cinema);
            if(data == null)
            {
                return null;
            }

            _cinemasRepository.Save();

            CinemaDomainModel domainModel = new CinemaDomainModel()
            {
                Id = data.Id,
                Name = data.Name
            };

            return domainModel;
        }

        public async Task<CinemaDomainModel> DeleteCinema(int id)
        {
            var data = _cinemasRepository.Delete(id);

            if(data == null)
            {
                return null;
            }

            _cinemasRepository.Save();

            CinemaDomainModel domainModel = new CinemaDomainModel()
            {
                Id = data.Id,
                Name = data.Name
            };

            return domainModel;
        }
    }

}
