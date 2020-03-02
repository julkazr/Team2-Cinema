using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;

namespace WinterWorkShop.Cinema.API.Models
{
    public class ReservationProcesModel
    {
        public Guid ProjectionId { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> SeatsToReserveID { get; set; }
    }
}
