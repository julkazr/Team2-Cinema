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
using WinterWorkShop.Cinema.Domain.Services;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILevi9PaymentService _levi9PaymentService;
        private readonly IUserService _userService;

        public ReservationsController(IReservationService reservationService, ILevi9PaymentService levi9PaymentService, IUserService userService)
        {
            _reservationService = reservationService;
            _levi9PaymentService = levi9PaymentService;
            _userService = userService;
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
                seatId = reservationModel.seatId,
                userId = reservationModel.userId
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

        //****************************************************************************************
        //RESERVATION PROCES
        
        [HttpPost]
        [Route("reserve")]
        public async Task<ActionResult<ReservationDomainModel>> ReservationProces(ReservationProcesModel model)
        {          
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //provera da li su sedista slobodna
            var reservationCheck = await _reservationService.CheckReservationForSeatsForProjection(model.SeatsToReserveID, model.ProjectionId);
            if(!reservationCheck.SeatsAreFree)
            {
                SeatTakenErrorResponseModel errorResponse = new SeatTakenErrorResponseModel
                {
                    ErrorMessage = reservationCheck.InfoMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    SeatsTakenID = reservationCheck.SeatsTaken
                };
                return BadRequest(errorResponse);
            }


            //provera placanja
            var paymentResponse = await _levi9PaymentService.MakePayment();
            if (!paymentResponse.IsSuccess)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = paymentResponse.Message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(errorResponse);
            }//ako je placanje uspesno:

            var userAfterBonusChange = await _userService.IncreaseBonus(model.UserId, model.SeatsToReserveID.Count);
            if(userAfterBonusChange == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = "Bonus failed to increase",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(errorResponse);
            }


            //izvrsi rezervaciju svakog sedista
            List<ReservationDomainModel> resultList = new List<ReservationDomainModel>();

            foreach (var seatId in model.SeatsToReserveID)
            {
                ReservationDomainModel reservation = new ReservationDomainModel
                {
                    seatId = seatId,
                    projectionId = model.ProjectionId,
                    userId = model.UserId
                };

                try
                {
                    var data = await _reservationService.AddReservation(reservation);
                    if(data != null)
                    {
                        reservation.id = data.id;
                        
                    }
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
                resultList.Add(reservation);
            }

            //Sve proslo
            return Created("reservations//reserve", resultList);
        }

        //CHECK FOR SEEAT POSITIONS BEFORE RESERVATION

        //[Authorize(Roles = "admin")]
        [HttpPost]
        [Route("check")]
        public async Task<ActionResult<CheckSeatsPositionDomainModel>> CheckSeatPositions(CheckSeetPositionsModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.listOfSeatsId.Count < 1 || model.listOfSeatsId == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = "List of id-seats is not valid",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(errorResponse);
            }


            CheckSeatsPositionDomainModel data;
            
            data = await _reservationService.CheckPositionBeforeReservation(model.listOfSeatsId);
            if (data == null)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = "Something went wrong in service",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(errorResponse);
            }

            if (!data.CheckSucceed)
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = data.InfoMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return BadRequest(errorResponse);
            }
            

            return Ok(data);
        }

    }
}