﻿using System.Collections.Generic;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class AuditoriumDomainModel
    {
        public int Id { get; set; }

        public int CinemaId { get; set; }

        public string Name { get; set; }

        public List<SeatDomainModel> SeatsList { get; set; }
    }
}
