﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.Domain.Interfaces
{
    public interface IProjectionService
    {
        Task<IEnumerable<ProjectionDomainModel>> GetAllAsync();
        Task<CreateProjectionResultModel> CreateProjection(ProjectionDomainModel domainModel);
        Task<IEnumerable<ProjectionDomainModel>> FilterProjections(FilterProjectionDomainModel filterProjectionDomainModel);
        Task<ProjectionDomainModel> DeleteProjection(Guid id);
        Task<ProjectionDomainModel> GetByIdAsync(Guid id);
        Task<ProjectionDomainModel> UpdateProjection(ProjectionDomainModel updateProjection);
        Task<IEnumerable<SeatDomainModel>> GetReserverdSeetsForProjection(Guid projectionId);
        /// <summary>
        /// Gets projection with auditorium for that projection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ProjectionWithAuditoriumResultModel> GetProjectionWithAuditorium(Guid id);
    }
}
