using System;
using System.Collections.Generic;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class CheckReservationForSeatsDomainModel
    {
        public bool SeatsAreFree { get; set; }
        public string InfoMessage { get; set; }
        public List<Guid> SeatsTaken { get; set; }

    }
}
