using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
