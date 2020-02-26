using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.API.Models
{
    public class UpdateProjectionModel
    {
        public Guid movieId { get; set; }

        public int auditoriumId { get; set; }

        public DateTime projectionTime { get; set; }

    }
}
