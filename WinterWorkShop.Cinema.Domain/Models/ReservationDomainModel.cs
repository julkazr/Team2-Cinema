using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class ReservationDomainModel
    {
        public int id { get; set; }

        public Guid seatId { get; set; }

        public Guid projectionId { get; set; }

        //public bool reservation { get; set; }
    }
}
