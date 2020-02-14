using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data
{
    [Table("projection")]
    public class Projection
    {
        public Guid Id { get; set; }

        [Column("auditoriumId")]
        public int AuditoriumId { get; set; }

        public DateTime DateTime { get; set; }

        [Column("movieId")]
        public Guid MovieId { get; set; }

        public virtual Movie Movie { get; set; }

        public virtual Auditorium Auditorium { get; set; }
    }
}
