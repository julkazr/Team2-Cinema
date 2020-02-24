﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data.Entities;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationService(IReservationRepository reserevationRepository)
        {
            _reservationRepository = reserevationRepository;
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
                    reservation = item.reservation,
                    seatId = item.seatId
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
                reservation = data.reservation
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
                reservation = newReservation.reservation
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
                reservation = data.reservation
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
                reservation = updateReservation.reservation
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
                reservation = data.reservation
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
                reservation = data.reservation,
                seatId = data.seatId
            };

            return domainModel;
        }
    }
}