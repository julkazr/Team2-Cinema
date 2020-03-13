using System.Collections.Generic;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class MovieProjectionsResultModel
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public IEnumerable<ProjectionDomainModel> Projections { get; set; }
        public AuditoriumDomainModel Auditorium { get; set; }
        public MovieDomainModel Movie { get; set; }

    }
}
