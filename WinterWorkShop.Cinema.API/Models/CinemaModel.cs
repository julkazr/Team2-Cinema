using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.API.Models
{
    public class CinemaModel
    {
        [Required]
        [StringLength(50, ErrorMessage = Messages.CINEMA_PROPERTIE_NAME_NOT_VALID)]
        public string Name { get; set; }
    }
}
