using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class UpdateAuditoriumModel
    {
        public string Name { get; set; }

        public int NumberOfSeats { get; set; }

        public int NumberOfRows { get; set; }
    }
}
