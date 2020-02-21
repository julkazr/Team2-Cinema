using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("tagMovies")]
    public class TagMovie
    {
        public int TagId { get; set; }
        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }
        public Tag Tag { get; set; }
    }
}
