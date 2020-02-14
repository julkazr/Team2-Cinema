using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data
{
    [Table("cinema")]
    public class Cinema
    {
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }

        public virtual ICollection<Auditorium> Auditoriums { get; set; }
    }
}
