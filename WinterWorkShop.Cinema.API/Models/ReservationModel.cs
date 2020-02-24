using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class ReservationModel
    {
        [Required]
        public Guid projectionId { get; set; }

        [Required]
        public Guid seatId { get; set; }

        public bool reservation { get; set; }
    }
}
