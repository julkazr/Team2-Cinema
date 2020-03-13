using System;
using System.ComponentModel.DataAnnotations;
using WinterWorkShop.Cinema.Domain.Common;

namespace WinterWorkShop.Cinema.API.Models
{
    public class MovieModel
    {
        [Required]
        [StringLength(50, ErrorMessage = Messages.MOVIE_PROPERTIE_TITLE_NOT_VALID)]
        public string Title { get; set; }

        [Required]
        [Range(1895, 2100, ErrorMessage = Messages.MOVIE_PROPERTIE_YEAR_NOT_VALID)]
        public int Year { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = Messages.MOVIE_PROPERTIE_RATING_NOT_VALID)]
        public double Rating { get; set; }

        public bool Current { get; set; }

        public bool Oscar { get; set; }
    }
}
