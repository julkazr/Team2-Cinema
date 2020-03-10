using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("ProjectionsWithMovieAndAuditorium")]
    public class ProjectionsWithMovieAndAuditorium
    {
        public Guid Id { get; set; }

        [Column("auditoriumId")]
        public int AuditoriumId { get; set; }

        public DateTime projectionDateTime { get; set; }

        [Column("movieId")]
        public Guid MovieId { get; set; }
        public string MovieTitle { get; set; }
        public int MovieYear { get; set; }
        public double? MovieRating { get; set; }
        public bool MovieCurrent { get; set; }
        public bool? MovieOscar { get; set; }
        public string AuditoriumName { get; set; }
    }
}
