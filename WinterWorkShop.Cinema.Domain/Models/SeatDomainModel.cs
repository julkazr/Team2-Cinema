using System;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class SeatDomainModel
    {
        public Guid Id { get; set; }

        public int AuditoriumId { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }
    }
}
