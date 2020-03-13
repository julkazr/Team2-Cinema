using System;
using System.Collections.Generic;
using System.Net;

namespace WinterWorkShop.Cinema.API.Models
{
    public class SeatTakenErrorResponseModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<Guid> SeatsTakenID { get; set; }
    }
}
