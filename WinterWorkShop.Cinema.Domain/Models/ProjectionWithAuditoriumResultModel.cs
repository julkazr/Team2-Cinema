using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class ProjectionWithAuditoriumResultModel
    {
        public int AuditoriumRowNumber { get; set; }
        public int AuditoriumSeatNumber { get; set; }
        public ProjectionDomainModel Projection { get; set; }
        public AuditoriumDomainModel Auditorium { get; set; }
        public MovieDomainModel Movie { get; set; }
        public List<SeatDomainModel> ListOfReservedSeats { get; set; }
    }
}
