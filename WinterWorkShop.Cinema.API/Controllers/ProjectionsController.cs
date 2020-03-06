using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectionsController : ControllerBase
    {
        private readonly IProjectionService _projectionService;

        public ProjectionsController(IProjectionService projectionService)
        {
            _projectionService = projectionService;
        }

        /// <summary>
        /// Gets all projections
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> GetAsync()
        {
            IEnumerable<ProjectionDomainModel> projectionDomainModels;
           
             projectionDomainModels = await _projectionService.GetAllAsync();            

            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainModel>();
            }

            return Ok(projectionDomainModels);
        }

        /// <summary>
        /// Adds a new projection
        /// </summary>
        /// <param name="projectionModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("")]
        public async Task<ActionResult<ProjectionDomainModel>> PostAsync(CreateProjectionModel projectionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (projectionModel.ProjectionTime < DateTime.Now)
            {
                ModelState.AddModelError(nameof(projectionModel.ProjectionTime), Messages.PROJECTION_IN_PAST);
                return BadRequest(ModelState);
            }

            ProjectionDomainModel domainModel = new ProjectionDomainModel
            {
                AuditoriumId = projectionModel.AuditoriumId,
                MovieId = projectionModel.MovieId,
                ProjectionTime = projectionModel.ProjectionTime
            };

            CreateProjectionResultModel createProjectionResultModel;

            try
            {
                createProjectionResultModel = await _projectionService.CreateProjection(domainModel);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (!createProjectionResultModel.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = createProjectionResultModel.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);                
            }

            return Created("projections//" + createProjectionResultModel.Projection.Id, createProjectionResultModel.Projection);
        }


        /// <summary>
        /// Filter projections
        /// </summary>
        /// <param name="cinemaId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("filter")]
        public async Task<ActionResult<IEnumerable<ProjectionDomainModel>>> GetFilteredProjections(FilterProjectionModel filterProjectionModel)
        {
            IEnumerable<ProjectionDomainModel> projectionDomainModels;

            FilterProjectionDomainModel filterProjectionDomainModel = new FilterProjectionDomainModel
            {
                cinemaId = filterProjectionModel.cinemaId,
                auditoriumId = filterProjectionModel.auditoriumId,
                movieId = filterProjectionModel.movieId,
                fromTime = filterProjectionModel.fromTime,
                toTime = filterProjectionModel.toTime
            };

            projectionDomainModels = await _projectionService.FilterProjections(filterProjectionDomainModel);

            //nema projekcija uopste
            if (projectionDomainModels == null)
            {
                projectionDomainModels = new List<ProjectionDomainModel>();
            }


            return Ok(projectionDomainModels);
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("update/{id}")]
        public async Task<ActionResult> Put(Guid id, UpdateProjectionModel updateProjectionModel)
        {       
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProjectionDomainModel projectionDomain = await _projectionService.GetByIdAsync(id);

            if (projectionDomain == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.PROJECTION_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            projectionDomain.MovieId = updateProjectionModel.movieId;
            projectionDomain.AuditoriumId = updateProjectionModel.auditoriumId;
            projectionDomain.ProjectionTime = updateProjectionModel.projectionTime;
            ProjectionDomainModel projectionDomainModel;

            try
            {
                projectionDomainModel = await _projectionService.UpdateProjection(projectionDomain);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Accepted("projections//" + projectionDomainModel.Id, projectionDomainModel);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            ProjectionDomainModel deletedProjection;
            try
            {
                deletedProjection = await _projectionService.DeleteProjection(id);
            }
            catch (DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if (deletedProjection == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.PROJECTION_DELETE_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("projections//" + deletedProjection.Id, deletedProjection);
        }



        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ProjectionDomainModel>> GetAsync(Guid id)
        {
            ProjectionDomainModel projection;

            projection = await _projectionService.GetByIdAsync(id);

            if (projection == null)
            {
                return NotFound(Messages.PROJECTION_DOES_NOT_EXIST);
            }
            return Ok(projection);
        }


        [HttpGet]
        [Route("reservedseats/{projectionId}")]
        public async Task<ActionResult<IEnumerable<SeatDomainModel>>> GetAllReservedSeats(Guid projectionId)
        {
            IEnumerable<SeatDomainModel> seatDomainModels;

            var reservedSeats = await _projectionService.GetReserverdSeetsForProjection(projectionId);

            if (reservedSeats == null)
            {
                reservedSeats = new List<SeatDomainModel>();
            }

            return Ok(reservedSeats);
        }


        /// <summary>
        /// Gets projection with auditorium for projection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getwithauditorium/{id}")]
        public async Task<ActionResult<ProjectionWithAuditoriumResultModel>> GetProjectionWithAuditorium(Guid id)
        {
            ProjectionWithAuditoriumResultModel projection;
            projection = await _projectionService.GetProjectionWithAuditorium(id);

            IEnumerable<SeatDomainModel> ReservedSeatsList = await _projectionService.GetReserverdSeetsForProjection(id);
            projection.ListOfReservedSeats = (List<SeatDomainModel>)ReservedSeatsList;

            if (projection == null)
            {
                return NotFound(Messages.PROJECTION_DOES_NOT_EXIST);
            }

            return Ok(projection);
        }

    }
}