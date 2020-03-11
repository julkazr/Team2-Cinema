using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatsRepository _seatsRepository;

        public SeatService(ISeatsRepository seatsRepository)
        {
            _seatsRepository = seatsRepository;
        }

        public async Task<IEnumerable<SeatDomainModel>> GetAllAsync()
        {
            var data = await _seatsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<SeatDomainModel> result = new List<SeatDomainModel>();
            SeatDomainModel model;
            foreach (var item in data)
            {
                model = new SeatDomainModel
                {
                    Id = item.Id,
                    AuditoriumId = item.AuditoriumId,
                    Number = item.Number,
                    Row = item.Row
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<SeatDomainModel> DeleteSeat(int id)
        {
            var data = _seatsRepository.Delete(id);

            if (data == null)
            {
                return null;
            }

            _seatsRepository.Save();

            SeatDomainModel domainModel = new SeatDomainModel()
            {
                Id = data.Id,
                AuditoriumId = data.AuditoriumId,
                Number = data.Number,
                Row = data.Row
            };

            return domainModel;
        }
    }
}
