using System;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class ReservationDomainModel
    {
        public int id { get; set; }

        public Guid seatId { get; set; }

        public Guid projectionId { get; set; }
        public Guid userId { get; set; }
    }
}
