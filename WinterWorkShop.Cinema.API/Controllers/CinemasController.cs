using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Interfaces;
using WinterWorkShop.Cinema.Domain.Models;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CinemasController : ControllerBase
    {
        private readonly ICinemaService _cinemaService;
        private readonly IAuditoriumService _auditoriumService;

        public CinemasController(ICinemaService cinemaService, IAuditoriumService auditoriumService)
        {
            _cinemaService = cinemaService;
            _auditoriumService = auditoriumService;
        }

        /// <summary>
        /// Gets all cinemas
        /// </summary>
        /// <returns>List of cinemas</returns>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<CinemaDomainModel>>> GetAsync()
        {
            IEnumerable<CinemaDomainModel> cinemaDomainModels;

            cinemaDomainModels = await _cinemaService.GetAllAsync();

            if (cinemaDomainModels == null)
            {
                cinemaDomainModels = new List<CinemaDomainModel>();
            }

            return Ok(cinemaDomainModels);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CinemaDomainModel>> GetByIdAsync(int id)
        {
            CinemaDomainModel cinema;

            cinema = await _cinemaService.GetByIdAsync(id);

            if (cinema == null)
            {
                return NotFound(Messages.CINEMA_DOES_NOT_EXIST);
            }

            return Ok(cinema);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Post([FromBody]CinemaModel cinemaModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CinemaDomainModel domainModel = new CinemaDomainModel
            {
                Name = cinemaModel.Name
            };

            CinemaDomainModel createCinema;

            try
            {
                createCinema = await _cinemaService.AddCinema(domainModel);
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

            if(createCinema == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Created("cinemas//" + createCinema.Id, createCinema);
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<ActionResult> Put(int id, [FromBody]CinemaModel cinemaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CinemaDomainModel cinemaToUpdate;

            cinemaToUpdate = await _cinemaService.GetByIdAsync(id);

            if (cinemaToUpdate == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            cinemaToUpdate.Name = cinemaModel.Name;

            CinemaDomainModel domainModel;

            try
            {
                domainModel = await _cinemaService.UpdateCinema(cinemaToUpdate);
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

            return Accepted("cinemas//" + domainModel.Id, domainModel);

        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            CinemaDomainModel deletedCinema;
            try
            {
                deletedCinema = await _cinemaService.DeleteCinema(id);
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

            if (deletedCinema == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_DOES_NOT_EXIST,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Accepted("cinemas//" + deletedCinema.Id, deletedCinema);
        }

        //**********************************************************************************
        //CREATE CINEMA WITH AUDITORIUM AND HIS SEATS
        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("createwithauditorium")]
        public async Task<ActionResult> PostCreateCinemaWithAuditorium([FromBody]CreateCinemaWithAuditoriumModel cinemaModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CinemaDomainModel domainModel = new CinemaDomainModel
            {
                Name = cinemaModel.cinemaName
            };

            CinemaDomainModel createdCinema;
            AuditoriumDomainModel createAuditorium;

            try
            {
                createdCinema = await _cinemaService.AddCinema(domainModel);
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

            if (createdCinema == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.CINEMA_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            //CREATE AUDITORIUM FOR JUST CREATED CINEMA

            AuditoriumDomainModel auditoriumToCreate = new AuditoriumDomainModel
            {
                CinemaId = createdCinema.Id,
                Name = cinemaModel.auditName
            };

            CreateAuditoriumResultModel createAuditoriumResult;

            try
            {
                createAuditoriumResult = await _auditoriumService.CreateAuditorium(auditoriumToCreate, cinemaModel.seatRows, cinemaModel.numberOfSeats);
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

            if (!createAuditoriumResult.IsSuccessful)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel()
                {
                    ErrorMessage = createAuditoriumResult.ErrorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            return Created("cinemas//" + createdCinema.Id, createdCinema);
        }



    }
}
