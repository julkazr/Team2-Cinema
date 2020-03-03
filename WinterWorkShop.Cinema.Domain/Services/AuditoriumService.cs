using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumsRepository _auditoriumsRepository;
        private readonly ICinemasRepository _cinemasRepository;
        private readonly ISeatsRepository _seatsRepository;

        public AuditoriumService(IAuditoriumsRepository auditoriumsRepository, ICinemasRepository cinemasRepository, ISeatsRepository seatsRepository)
        {
            _auditoriumsRepository = auditoriumsRepository;
            _cinemasRepository = cinemasRepository;
            _seatsRepository = seatsRepository;
        }

        public async Task<IEnumerable<AuditoriumDomainModel>> GetAllAsync()
        {
            var data = await _auditoriumsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<AuditoriumDomainModel> result = new List<AuditoriumDomainModel>();
            AuditoriumDomainModel model;
            foreach (var item in data)
            {
                model = new AuditoriumDomainModel
                {
                    Id = item.Id,
                    CinemaId = item.CinemaId,
                    Name = item.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CreateAuditoriumResultModel> CreateAuditorium(AuditoriumDomainModel domainModel, int numberOfRows, int numberOfSeatsPerRow)
        {
            var cinema = await _cinemasRepository.GetByIdAsync(domainModel.CinemaId);
            if (cinema == null)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_UNVALID_CINEMAID
                };
            }

            var auditorium = await _auditoriumsRepository.GetByAuditName(domainModel.Name, domainModel.CinemaId);
            var sameAuditoriumName = auditorium.ToList() ?? new List<Auditorium>();
            if (sameAuditoriumName != null && sameAuditoriumName.Count > 0)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_SAME_NAME
                };
            }

            Auditorium newAuditorium = new Auditorium
            {
                Name = domainModel.Name,
                CinemaId = domainModel.CinemaId,
            };

            newAuditorium.Seats = new List<Seat>();

            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= numberOfSeatsPerRow; j++)
                {
                    Seat newSeat = new Seat()
                    {
                        Row = i,
                        Number = j
                    };

                    newAuditorium.Seats.Add(newSeat);
                }
            }

            Auditorium insertedAuditorium = _auditoriumsRepository.Insert(newAuditorium);
            if (insertedAuditorium == null)
            {
                return new CreateAuditoriumResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.AUDITORIUM_CREATION_ERROR
                };
            }

            CreateAuditoriumResultModel resultModel = new CreateAuditoriumResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Auditorium = new AuditoriumDomainModel
                {
                    Id = insertedAuditorium.Id,
                    Name = insertedAuditorium.Name,
                    CinemaId = insertedAuditorium.CinemaId,
                    SeatsList = new List<SeatDomainModel>()
                }
            };

            foreach (var item in insertedAuditorium.Seats)
            {
                resultModel.Auditorium.SeatsList.Add(new SeatDomainModel
                {
                    AuditoriumId = insertedAuditorium.Id,
                    Id = item.Id,
                    Number = item.Number,
                    Row = item.Row
                });
            }

            _auditoriumsRepository.Save();

            return resultModel;
        }

        public async Task<AuditoriumDomainModel> GetByIdAsync(int id)
        {
            var data = await _auditoriumsRepository.GetByIdAsync(id);

            if (data == null)
            {
                return null;
            }

            AuditoriumDomainModel domainModel = new AuditoriumDomainModel()
            {
                Id = data.Id,
                Name = data.Name,
                CinemaId = data.CinemaId,
                SeatsList = new List<SeatDomainModel>()
            };

            foreach(var item in data.Seats)
            {
                domainModel.SeatsList.Add(new SeatDomainModel
                {
                    AuditoriumId = data.Id,
                    Id = item.Id,
                    Number = item.Number,
                    Row = item.Row
                });
            }

            return domainModel;
        }



        public async Task<AuditoriumDomainModel> UpdateAuditorium(AuditoriumDomainModel auditoriumDomain, int numberOfRows, int numberOfSeats, bool SeatsAreFree)
        {
            Auditorium auditorium = new Auditorium()
            { 
                Id = auditoriumDomain.Id,
                CinemaId = auditoriumDomain.CinemaId,
                Name = auditoriumDomain.Name,
                Seats = new List<Seat>()
            };

            Auditorium update = new Auditorium();
            update.Seats = new List<Seat>();
            update = await _auditoriumsRepository.GetByIdAsync(auditorium.Id);
            update.Name = auditorium.Name;
            int number = new int();
            int row = new int();
            List<Seat> newSeats = new List<Seat>();
            foreach (var item in update.Seats)
            {
                newSeats.Add(item);
                number = item.Number;
                row = item.Row;
            }

            if (numberOfRows > row)
            {
                for (int i = row + 1; i <= numberOfRows; i++)
                {
                    for (int j = 1; j <= number; j++)
                    {
                        Seat seat = new Seat
                        {
                            Number = j,
                            Row = i
                        };
                        update.Seats.Add(seat);
                    }
                }
                row = numberOfRows;
            }
            if (numberOfRows < row)
            {
                List<Seat> seats = new List<Seat>();
                if (!SeatsAreFree)
                {
                    return null;
                }
                foreach (var item in update.Seats)
                {
                    if (item.Row > numberOfRows)
                    {
                        seats.Add(item);
                    }
                }
                foreach (var item in seats)
                {
                    update.Seats.Remove(item);
                    Seat seat = new Seat();
                    seat = _seatsRepository.Delete(item.Id);
                    _seatsRepository.Save();
                }
                row = numberOfRows;
            }

            var data = _auditoriumsRepository.Update(update);
            if (data == null)
            {
                return null;
            }

            _auditoriumsRepository.Save();

            if (numberOfSeats > number)
            {
                for (int i = 1; i <= row; i++)
                {
                    for (int j = number + 1; j <= numberOfSeats; j++)
                    {
                        Seat seat = new Seat
                        {
                            Number = j,
                            Row = i
                        };
                        update.Seats.Add(seat);
                        Seat newSeat = new Seat();
                        newSeat = _seatsRepository.Insert(seat);
                    }
                }
                number = numberOfSeats;
            }
            if (numberOfSeats < number)
            {
                if (!SeatsAreFree)
                {
                    return null;
                }
                List<Seat> seats = new List<Seat>();
                foreach (var item in update.Seats)
                {
                    if (item.Number > numberOfSeats)
                    {
                        seats.Add(item);
                    }
                }
                foreach(var item in seats)
                {
                    update.Seats.Remove(item);
                    Seat seat = _seatsRepository.Delete(item.Id);
                    _seatsRepository.Save();
                }
                number = numberOfSeats;
            }

            data = _auditoriumsRepository.Update(update);
            if (data == null)
            {
                return null;
            }

            _auditoriumsRepository.Save();

            AuditoriumDomainModel domainModel = new AuditoriumDomainModel()
            {
                Id = data.Id,
                Name = data.Name,
                CinemaId = data.CinemaId
            };
            domainModel.SeatsList = new List<SeatDomainModel>();

            foreach(var item in data.Seats)
            {
                SeatDomainModel seat = new SeatDomainModel
                {
                    AuditoriumId = item.AuditoriumId,
                    Id = item.Id,
                    Number = item.Number,
                    Row = item.Row
                };
                domainModel.SeatsList.Add(seat);
            }

            return domainModel;
        }

        public async Task<AuditoriumDomainModel> DeleteAuditorium(int id)
        {
            var data = await _auditoriumsRepository.GetByIdAsync(id);
            List<Seat> seats = new List<Seat>();
            List<Seat> seatsResult = new List<Seat>();
            foreach (var item in data.Seats)
            {
                seats.Add(item);
            }

            for (int i = 0; i < seats.Count; i ++)
            {
                Seat seatsDelete = _seatsRepository.Delete(seats[i].Id);
                seatsResult.Add(seatsDelete);
  
            }
            _seatsRepository.Save();

            var result = _auditoriumsRepository.Delete(id);

            if (result == null)
            {
                return null;
            }

            _auditoriumsRepository.Save();

            AuditoriumDomainModel domainModel = new AuditoriumDomainModel()
            {
                Id = data.Id,
                Name = data.Name,
                CinemaId = data.CinemaId
            };

            return domainModel;
        }
    }
}
