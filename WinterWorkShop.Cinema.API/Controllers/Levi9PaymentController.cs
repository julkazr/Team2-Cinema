using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.Domain.Common;
using WinterWorkShop.Cinema.Domain.Models;
using WinterWorkShop.Cinema.Domain.Services;

namespace WinterWorkShop.Cinema.API.Controllers
{
    // DO NOT TOUCH
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class Levi9PaymentController : ControllerBase
    {
        private readonly ILevi9PaymentService _paymentService;

        public Levi9PaymentController(ILevi9PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<PaymentResponseModel>> Post()
        {
            PaymentResponse paymentResult;

            paymentResult = await _paymentService.MakePayment();
            
            if (paymentResult.Message != "Connection error.")
            {
                PaymentResponseModel model = new PaymentResponseModel
                {
                    IsSuccess = paymentResult.IsSuccess,
                    Message = paymentResult.Message
                };

                if (!model.IsSuccess)
                {
                    return BadRequest(model);
                }

                return Ok(model);
            }
            else 
            {
                ErrorResponseModel errorResponse = new ErrorResponseModel
                {
                    ErrorMessage = Messages.PAYMENT_CREATION_ERROR,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

                return BadRequest(errorResponse);
            }
        }
    }
}