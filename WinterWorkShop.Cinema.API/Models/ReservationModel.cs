using System;
using System.ComponentModel.DataAnnotations;

namespace WinterWorkShop.Cinema.API.Models
{
    public class ReservationModel
    {
        [Required]
        public Guid projectionId { get; set; }

        [Required]
        public Guid seatId { get; set; }

        public Guid userId { get; set; }
    }
}
