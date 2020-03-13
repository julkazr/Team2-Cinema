using System;
using System.Collections.Generic;

namespace WinterWorkShop.Cinema.API.Models
{
    public class ReservationProcesModel
    {
        public Guid ProjectionId { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> SeatsToReserveID { get; set; }
    }
}
