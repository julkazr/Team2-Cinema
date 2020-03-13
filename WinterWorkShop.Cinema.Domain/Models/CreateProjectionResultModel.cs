namespace WinterWorkShop.Cinema.Domain.Models
{
    public class CreateProjectionResultModel
    {
        public ProjectionDomainModel Projection { get; set; }

        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; }
    }
}
