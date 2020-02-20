using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Repositories;

namespace WinterWorkShop.Cinema.Domain.Services
{
    public class ProjectionService : IProjectionService
    {
        private readonly IProjectionsRepository _projectionsRepository;
        private readonly ICinemasRepository _cinemasRepository;

        public ProjectionService(IProjectionsRepository projectionsRepository, ICinemasRepository cinemasRepository)
        {
            _projectionsRepository = projectionsRepository;
            _cinemasRepository = cinemasRepository;
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

    }
}
