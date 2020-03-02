using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ISeatsRepository _seatsRepository;

        public ReservationService(IReservationRepository reserevationRepository, ISeatsRepository seatsRepository)
        {
            _reservationRepository = reserevationRepository;
            _seatsRepository = seatsRepository;
        }

        public async Task<IEnumerable<ReservationDomainModel>> GetAllAsync()
        {
            var data = await _reservationRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<ReservationDomainModel> result = new List<ReservationDomainModel>();
            ReservationDomainModel model;
            foreach (var item in data)
            {
                model = new ReservationDomainModel
                {
                    id = item.id,
                    projectionId = item.projectionId,
                    //reservation = item.reservation,
                    seatId = item.seatId,
                    userId = item.userId
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<ReservationDomainModel> GetByIdAsync(int id)
        {
            var data = await _reservationRepository.GetByIdAsync(id);

            if(data == null)
            {
                return null;
            }

            ReservationDomainModel reservationDomainModel = new ReservationDomainModel
            {
                id = data.id,
                projectionId = data.projectionId,
                seatId = data.seatId,
                //reservation = data.reservation
                userId = data.userId
            };

            return reservationDomainModel;
        }

        public async Task<ReservationDomainModel> AddReservation(ReservationDomainModel newReservation)
        {
            Reservation reservationToCreate = new Reservation
            {
                id = newReservation.id,
                projectionId = newReservation.projectionId,
                seatId = newReservation.seatId,
                //reservation = newReservation.reservation
                userId = newReservation.userId
            };

            var data = _reservationRepository.Insert(reservationToCreate);

            if(data == null)
            {
                return null;
            }

            _reservationRepository.Save();

            ReservationDomainModel domainModel = new ReservationDomainModel
            {
                id = data.id,
                projectionId = data.projectionId,
                seatId = data.seatId,
                //reservation = data.reservation
                userId = data.userId
            };

            return domainModel;
        }

        public async Task<ReservationDomainModel> UpdateReservation(ReservationDomainModel updateReservation)
        {
            Reservation reservationToUpdate = new Reservation
            {
                id = updateReservation.id,
                projectionId = updateReservation.projectionId,
                seatId = updateReservation.seatId,
                //reservation = updateReservation.reservation
                userId = updateReservation.userId
            };

            var data = _reservationRepository.Update(reservationToUpdate);

            if(data == null)
            {
                return null;
            }

            _reservationRepository.Save();

            ReservationDomainModel domainModel = new ReservationDomainModel
            {
                id = data.id,
                projectionId = data.projectionId,
                seatId = data.seatId,
                //reservation = data.reservation
                userId = data.userId
            };

            return domainModel;
        }

        public async Task<ReservationDomainModel> DeleteReservation(int id)
        {
            var data = _reservationRepository.Delete(id);

            if(data == null)
            {
                return null;
            }

            _reservationRepository.Save();

            ReservationDomainModel domainModel = new ReservationDomainModel
            {
                id = data.id,
                projectionId = data.projectionId,
                //reservation = data.reservation,
                seatId = data.seatId,
                userId = data.userId
                
            };

            return domainModel;
        }
        //****************************************************************************


        //Provera da li su sedista vece rezervisana
        public async Task<CheckReservationForSeatsDomainModel> CheckReservationForSeats(List<Guid> listOfSeatsId)
        {
            CheckReservationForSeatsDomainModel result = new CheckReservationForSeatsDomainModel();
            result.SeatsTaken = new List<Guid>();
            var allReserevations = await _reservationRepository.GetAll();

            //provera da li uopste ima rezervacija
            if (allReserevations.Count() == 0)
            {
                result.SeatsAreFree = true;
                result.InfoMessage = "There are no reservations";
                return result;
            }

            //provera da li su sedista zauzeta
            foreach (var seatId in listOfSeatsId)
            {
                var reservationForGivenSeat = allReserevations.Where(x => x.seatId.Equals(seatId)).ToList();
                if (reservationForGivenSeat.Count > 0)
                {
                    result.SeatsTaken.Add(seatId);                    
                }
            }
            if(result.SeatsTaken.Count > 0)
            {
                result.SeatsAreFree = false;
                result.InfoMessage = "Some of seats are already reserved";
                return result;
            }


            result.SeatsAreFree = true;
            result.InfoMessage = "Seats are free to reserve";
            return result;

        }
        //****************************************************************************


        //Provera da li sedista zadovoljavaju kriterijume: granicne vrednosti i redno rezervisnanje

        public async Task<CheckSeatsPositionDomainModel> CheckPositionBeforeReservation(List<Guid> listOfSeatsId)
        {
            Seat seat;
            int currentAuditoriumId;

            if (listOfSeatsId.Count() == 0)
            {
                return null;
            }

            var n = listOfSeatsId[0];
            seat = await _seatsRepository.GetByGuid(n);
            currentAuditoriumId = seat.AuditoriumId;

            var listOFAllSeats = await _seatsRepository.GetAll();
            var seatsOfGivenAudiorium = listOFAllSeats.Where(x => x.AuditoriumId.Equals(currentAuditoriumId));

            //maximalne vrednosti redova i sedista
            int AuditoriumMaxRow = seatsOfGivenAudiorium.Max(x => x.Row);
            int AuditoriumMAxNumber = seatsOfGivenAudiorium.Max(x => x.Number);


            //lista sedista za poroveru
            List<Seat> selectedSeats = new List<Seat>();

            foreach (var item in listOfSeatsId)
            {
                Seat selectedSeat = await _seatsRepository.GetByIdAsync(item);
                selectedSeats.Add(selectedSeat);
            }

            //OK LISTA
            List<Seat> okList = new List<Seat>();
            CheckSeatsPositionDomainModel result = new CheckSeatsPositionDomainModel();

            //ako pokusava da se rezervise vise od jednog sedista proveri da li su jedno do drugog i da li prelazi u drugi red
            if (selectedSeats.Count > 1)
            {
                Seat firstSeat = selectedSeats[0];
                okList.Add(firstSeat);
                
                int currentRow = okList.Last().Row;

                int j = 1;
                for (int i = 1; i < selectedSeats.Count; i++)
                {

                    if (!(selectedSeats[i].Number == okList.First().Number - 1 || selectedSeats[i].Number == okList.Last().Number + 1))
                    {
                        //return sedista nisu u nizu 
                        if ((selectedSeats[i].Row == firstSeat.Row +1)  &&  (selectedSeats[i].Number == j) && (selectedSeats[i-j].Number == AuditoriumMAxNumber))
                        {

                            okList.Add(selectedSeats[i]);
                            j++;
                            currentRow = selectedSeats[i].Row;

                            //Sedista prelaze u drugi red ali su u nizu
                            result.SeetExceedingRow = true;
                            result.InfoMessage = "Seets escceding one row";
                            
                        }
                        //Sedista nisu u nizu ni u redu
                        result.CheckSucceed = false;
                        result.SeetExceedingRow = true;
                        result.InfoMessage = "Seets are not next to each other and they are not in same row";
                        return result;
                    }
                    else
                    {
                        //sediste je u nizu     
                        if (selectedSeats[i].Row == currentRow)
                        {
                            //Sediste je u nizu u odgovarajucem redu
                            okList.Add(selectedSeats[i]);
                        }
                        else
                        {
                            //Sediste je u nizu ali ne u odgovarajucem redu - resava bug
                            result.CheckSucceed = false;
                            result.SeetExceedingRow = true;
                            result.InfoMessage = "Seets are not next to each other and exceeding the row";
                            return result;
                        }

                        //okList.Add(selectedSeats[i]);
                    }
                }

                //Sva sedista su ok
                result.CheckSucceed = true;
                if(result.InfoMessage == null)
                {
                    result.InfoMessage = "Seets are next to each other and they are in same row";
                }
            }
            else
            {
                //Samo jedno sediste se rezervise i ono mora biti ok
                okList.Add(selectedSeats[0]);
                result.CheckSucceed = true;
                result.InfoMessage = "You passed only one seet";
            }

            return result;
        }

    }
}
