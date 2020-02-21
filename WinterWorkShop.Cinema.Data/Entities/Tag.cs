using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("tag")]
    public class Tag
    {
        public int Id { get; set; }

        public string TagContent { get; set; }

        public virtual ICollection<TagMovie> TagMovies { get; set; }

    }
}
