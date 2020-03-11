﻿using System;
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
    public class ProjectionService : IProjectionService
    {
        private readonly IProjectionsRepository _projectionsRepository;
        private readonly IReservationRepository _reservationRepository;

        public ProjectionService(IProjectionsRepository projectionsRepository, IReservationRepository reservationRepository)
        {
            _projectionsRepository = projectionsRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<ProjectionDomainModel>> GetAllAsync()
        {
            var data = await _projectionsRepository.GetAll();

            if (data == null)
            {
                return null;
            }

            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in data)
            {
                model = new ProjectionDomainModel
                {
                    Id = item.Id,
                    MovieId = item.MovieId,
                    AuditoriumId = item.AuditoriumId,
                    ProjectionTime = item.DateTime,
                    MovieTitle = item.Movie.Title,
                    AditoriumName = item.Auditorium.Name
                };
                result.Add(model);
            }

            return result;
        }

        public async Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel)
        {
            int projectionTime = 3;

            var projectionsAtSameTime = _projectionsRepository.GetByAuditoriumId(domainModel.AuditoriumId)
                .Where(x => x.DateTime < domainModel.ProjectionTime.AddHours(projectionTime) && x.DateTime > domainModel.ProjectionTime.AddHours(-projectionTime))
                .ToList();

            if (projectionsAtSameTime != null && projectionsAtSameTime.Count > 0)
            {
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTIONS_AT_SAME_TIME
                };
            }

            var newProjection = new Data.Projection
            {
                MovieId = domainModel.MovieId,
                AuditoriumId = domainModel.AuditoriumId,
                DateTime = domainModel.ProjectionTime
            };

            var insertedProjection = _projectionsRepository.Insert(newProjection);

            if (insertedProjection == null)
            {
                return new CreateProjectionResultModel
                {
                    IsSuccessful = false,
                    ErrorMessage = Messages.PROJECTION_CREATION_ERROR
                };
            }

            _projectionsRepository.Save();
            CreateProjectionResultModel result = new CreateProjectionResultModel
            {
                IsSuccessful = true,
                ErrorMessage = null,
                Projection = new ProjectionDomainModel
                {
                    Id = insertedProjection.Id,
                    AuditoriumId = insertedProjection.AuditoriumId,
                    MovieId = insertedProjection.MovieId,
                    ProjectionTime = insertedProjection.DateTime
                }
            };

            return result;
        }
        
        public async Task<IEnumerable<ProjectionDomainModel>> FilterProjections(FilterProjectionDomainModel filterProjectionDomainModel)
        {
            var allProjections = await _projectionsRepository.GetAll();
            //ako nema projekcija
            if (allProjections == null)
            {
                return null;
            }

            //FILTER
            //By cinema
            if (filterProjectionDomainModel.cinemaId != null)
            {
                var tempProjections = allProjections.Where(x => x.Auditorium.CinemaId.Equals(filterProjectionDomainModel.cinemaId)).ToList();
                allProjections = tempProjections;
            }
            //By auditorium
            if (filterProjectionDomainModel.auditoriumId != null)
            {
                var tempProjections = allProjections.Where(x => x.AuditoriumId == filterProjectionDomainModel.auditoriumId);
                allProjections = tempProjections;
            }
            //By Movie
            if(filterProjectionDomainModel.movieId != null)
            {
                var tempProjections = allProjections.Where(x => x.MovieId == filterProjectionDomainModel.movieId);
                allProjections = tempProjections;
            }
            //By Date span
            var newList = allProjections.Where(projection =>
                        (
                            filterProjectionDomainModel.fromTime == null
                            || filterProjectionDomainModel.fromTime <= projection.DateTime
                        )
                        &&
                        (
                            filterProjectionDomainModel.toTime == null
                            || filterProjectionDomainModel.toTime >= projection.DateTime
                        )

                    ).ToList();


            //prepakivanje u domainmodel
            List<ProjectionDomainModel> result = new List<ProjectionDomainModel>();
            ProjectionDomainModel model;
            foreach (var item in newList)
            {
                model = new ProjectionDomainModel
                {
                    Id = item.Id,
                    MovieId = item.MovieId,
                    AuditoriumId = item.AuditoriumId,
                    ProjectionTime = item.DateTime,
                    MovieTitle = item.Movie.Title,
                    AditoriumName = item.Auditorium.Name
                };
                result.Add(model);
            }


            return result;
        }

        //DELETE PROJECTION
        public async Task<ProjectionDomainModel> DeleteProjection(Guid id)
        {

            Projection existing = await _projectionsRepository.GetByIdWithReservationAsync(id);
            if (existing == null)
            {
                //proveri sta trebad a vratis
                return null;
            }
            if (existing.Reservations.Count != 0)
            {
                var reservationsForProjection = existing.Reservations;

                foreach (var item in reservationsForProjection)
                {
                    var reservationDeleted = _reservationRepository.Delete(item.id);
                    if (reservationDeleted == null)
                    {
                        return null;
                    }
                }
                _reservationRepository.Save();
            }

            
            //da li moze save nakon svih


            //brise projekciju
            var data = _projectionsRepository.Delete(existing.Id);

            if (data == null)
            {
                return null;
            }
            _projectionsRepository.Save();


            ProjectionDomainModel domainModel = new ProjectionDomainModel()
            {
                Id = data.Id,
                MovieId = data.MovieId,
                AuditoriumId = data.AuditoriumId,
                ProjectionTime = data.DateTime
            };

            return domainModel;
        }

        public async Task<ProjectionDomainModel> GetByIdAsync(Guid id)
        {
            //var data = await _projectionsRepository.GetByIdAsync(id);
            var data = await _projectionsRepository.GetByIdAsyncView(id);
            //view

            if (data == null)
            {
                return null;
            }

            ProjectionDomainModel domainModel = new ProjectionDomainModel();
            domainModel = new ProjectionDomainModel
            {
                Id = data.Id,
                AuditoriumId = data.AuditoriumId,
                MovieId = data.MovieId,
                ProjectionTime = data.DateTime,
                AditoriumName = data.Auditorium.Name,
                MovieTitle = data.Movie.Title
            };

            return domainModel;
        }

        public async Task<ProjectionDomainModel> UpdateProjection(ProjectionDomainModel updateProjection)
        {
            Projection projection = new Projection()
            {
                Id = updateProjection.Id,
                AuditoriumId = updateProjection.AuditoriumId,
                MovieId = updateProjection.MovieId,
                DateTime = updateProjection.ProjectionTime
            };

            var data = _projectionsRepository.Update(projection);
            if (data == null)
            {
                return null;
            }

            _projectionsRepository.Save();

            ProjectionDomainModel domainModel = new ProjectionDomainModel()
            {
                Id = data.Id,
                MovieId = data.MovieId,
                AuditoriumId = data.AuditoriumId,
                ProjectionTime = data.DateTime
            };

            return domainModel;
        }

        public async Task<ProjectionWithAuditoriumResultModel> GetProjectionWithAuditorium(Guid id)
        {
            var projectionWithAuditorium = await _projectionsRepository.GetByIdWithAuditoriumIncluded(id);
            if(projectionWithAuditorium == null)
            {
                return null;
            }
            var maxRow = 0;
            var maxNum = 0;
            Seat maxByNum = projectionWithAuditorium.Auditorium.Seats.First();
            Seat maxByRow = projectionWithAuditorium.Auditorium.Seats.First();
            SeatDomainModel seatForAuditorium;
            var seats = new List<SeatDomainModel>();

            foreach (var seat in projectionWithAuditorium.Auditorium.Seats)
            {
                if (seat.Row > maxByRow.Row)
                {
                    maxByRow = seat;
                    maxRow = maxByRow.Row;
                }

                if (seat.Number > maxByNum.Number)
                {
                    maxByNum = seat;
                    maxNum = maxByNum.Number;
                }


                seatForAuditorium = new SeatDomainModel
                {
                    Id = seat.Id,
                    Number = seat.Number,
                    Row = seat.Row
                };
                seats.Add(seatForAuditorium);
            } 

            ProjectionWithAuditoriumResultModel projModel = new ProjectionWithAuditoriumResultModel
            {
                //IsSuccessful = true,
                //ErrorMessage = null,
                Projection = new ProjectionDomainModel
                {
                    Id = projectionWithAuditorium.Id,
                    AditoriumName = projectionWithAuditorium.Auditorium.Name,
                    AuditoriumId = projectionWithAuditorium.AuditoriumId,

                    MovieId = projectionWithAuditorium.MovieId,
                    MovieTitle = projectionWithAuditorium.Movie.Title,
                    ProjectionTime = projectionWithAuditorium.DateTime
                },
                Auditorium = new AuditoriumDomainModel
                {
                    CinemaId = projectionWithAuditorium.Auditorium.CinemaId,
                    SeatsList = seats
                },
                Movie = new MovieDomainModel
                {
                    Rating = projectionWithAuditorium.Movie.Rating ?? 0,
                    Year = projectionWithAuditorium.Movie.Year
                },
                AuditoriumRowNumber = maxRow,
                AuditoriumSeatNumber = maxNum
            };
            return projModel;
        }

        //Get all taken seets for auditorium
        public async Task<IEnumerable<SeatDomainModel>> GetReserverdSeetsForProjection(Guid projectionId)
        {
            List<SeatDomainModel> listOfReservedSeats = new List<SeatDomainModel>();
            //_reservationRepository

            var data = await _projectionsRepository.GetByIdWithReservationsAsync(projectionId);

            if (data == null)
            {
                return null;
            }

            var reservationsForSeletedProjection = data.Reservations;
            foreach (var reservation in reservationsForSeletedProjection)
            {
                SeatDomainModel seatDomainModel = new SeatDomainModel
                {
                    Id = reservation.Seat.Id,
                    AuditoriumId = reservation.Seat.AuditoriumId,
                    Row = reservation.Seat.Row,
                    Number = reservation.Seat.Number
                };
                listOfReservedSeats.Add(seatDomainModel);
            }

            return listOfReservedSeats;
        }


    }
}
