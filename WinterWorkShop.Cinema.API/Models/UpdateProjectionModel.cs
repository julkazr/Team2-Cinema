using System;

namespace WinterWorkShop.Cinema.API.Models
{
    public class UpdateProjectionModel
    {
        public Guid movieId { get; set; }

        public int auditoriumId { get; set; }

        public DateTime projectionTime { get; set; }

    }
}
