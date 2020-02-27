using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class CheckReservationForSeatsDomainModel
    {
        public bool SeatsAreFree { get; set; }
        public string InfoMessage { get; set; }
        public List<Guid> SeatsTaken { get; set; }

    }
}
