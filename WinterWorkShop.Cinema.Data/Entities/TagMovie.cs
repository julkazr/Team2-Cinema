using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("tagMovie")]
    public class TagMovie
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public Guid MovieId { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
