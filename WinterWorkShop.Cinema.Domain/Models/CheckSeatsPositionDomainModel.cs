using System;
using System.Collections.Generic;
using System.Text;

namespace WinterWorkShop.Cinema.Domain.Models
{
    public class CheckSeatsPositionDomainModel
    {
        public bool CheckSucceed { get; set; }
        public bool SeetExceedingRow { get; set; }
        public string InfoMessage { get; set; }
        
    }
}
