using System;
using System.Collections.Generic;
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
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<ReservationDomainModel>>> getAsync()
        {
            IEnumerable<ReservationDomainModel> reservationDomainModels;

            reservationDomainModels = await _reservationService.GetAllAsync();

            if (reservationDomainModels == null)
            {
                reservationDomainModels = new List<ReservationDomainModel>();
            }

            return Ok(reservationDomainModels);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ReservationDomainModel>> getByIdAsync(int id)
        {
            ReservationDomainModel reservation;
            reservation = await _reservationService.GetByIdAsync(id);


            if (reservation == null)
            {
                return NotFound(Messages.RESERVATION_DOES_NOT_EXIST);
            }

            return Ok(reservation);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Post([FromBody]ReservationModel reservationModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ReservationDomainModel domainModel = new ReservationDomainModel
            {
                projectionId = reservationModel.projectionId,
                reservation = reservationModel.reservation,
                seatId = reservationModel.seatId
            };

            ReservationDomainModel createReservation;

            try
            {
                createReservation = await _reservationService.AddReservation(domainModel);
            }
            catch(DbUpdateException e)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = e.InnerException.Message ?? e.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }

            if(createReservation == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.RESERVATION_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, errorResponse);
            }

            return Created("reservations//" + createReservation.id, createReservation);
        }
    }
}