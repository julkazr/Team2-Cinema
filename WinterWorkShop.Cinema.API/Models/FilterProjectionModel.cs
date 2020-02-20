using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class FilterProjectionModel
    {
        public int? cinemaId { get; set; }
        public int? auditoriumId { get; set; }
        public Guid? movieId { get; set; }
        public DateTime? fromTime { get; set; }
        public DateTime? toTime { get; set; }

    }
}
