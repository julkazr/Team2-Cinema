using System;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class ProjectionDomainModel
    {
        public Guid Id { get; set; }

        public Guid MovieId { get; set; }

        public string MovieTitle { get; set; }

        public int AuditoriumId { get; set; }

        public string AditoriumName { get; set; }

        public DateTime ProjectionTime { get; set; }
    }
}
