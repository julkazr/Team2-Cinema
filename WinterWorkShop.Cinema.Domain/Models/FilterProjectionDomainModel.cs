using System;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class FilterProjectionDomainModel
    {
        public int? cinemaId { get; set; }
        public int? auditoriumId { get; set; }
        public Guid? movieId { get; set; }
        public DateTime? fromTime { get; set; }
        public DateTime? toTime { get; set; }
    }
}
