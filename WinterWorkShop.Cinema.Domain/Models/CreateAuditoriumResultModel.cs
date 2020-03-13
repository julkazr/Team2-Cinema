﻿namespace WinterWorkShop.Cinema.Domain.Models
{
    public class CreateAuditoriumResultModel
    {
        public bool IsSuccessful { get; set; }

        public string ErrorMessage { get; set; }

        public AuditoriumDomainModel Auditorium { get; set; }

    }
}
