using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.API.Models;
using WinterWorkShop.Cinema.API.TokenServiceExtensions;
using WinterWorkShop.Cinema.Domain.Interfaces;

namespace WinterWorkShop.Cinema.API.Controllers
{
    [OpenApiIgnore]
    public class DemoAuthenticationController : ControllerBase    
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public DemoAuthenticationController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        // NOT FOR PRODUCTION USE!!!
        // you will need a robust auth implementation for production
        // i.e. try IdentityServer4
        [HttpGet]
        [Route("/get-token")]
        public async Task<IActionResult> GenerateToken(string name = "aspnetcore-workshop-demo", bool admin = false, bool superUser = false)
        {
            try
            {
                var user = await _userService.GetUserByUserName(name);
                admin = user.IsAdmin;
                superUser = user.IsSuperUser;
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
            var jwt = JwtTokenGenerator
                .Generate(name, admin, superUser, _configuration["Tokens:Issuer"], _configuration["Tokens:Key"]);

            return Ok(new {token = jwt});
        }
    }
}
