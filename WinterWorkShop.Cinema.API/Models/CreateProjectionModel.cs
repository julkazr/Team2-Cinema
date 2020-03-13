using System;
using System.ComponentModel.DataAnnotations;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CreateProjectionModel
    {
        [Required]
        [Range(1, Int32.MaxValue)]
        public int AuditoriumId { get; set; }

        [Required]
        public DateTime ProjectionTime { get; set; }

        [Required]
        public Guid MovieId { get; set; }
    }
}
