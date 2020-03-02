using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class ProjectionWithAuditoriumResultModel
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public int AuditoriumRowNumber { get; set; }
        public int AuditoriumSeatNumber { get; set; }
        public ProjectionDomainModel Projection { get; set; }

        //public IEnumerable<SeatDomainModel> Seats { get; set; }
    }
}
