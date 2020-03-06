using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Data
{
    [Table("user")]
    public class User
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Column("userName")]
        public string UserName { get; set; }        

        [Column("isAdmin")]
        public bool IsAdmin { get; set; }

        [Column("bonus")]
        public int? bonus { get; set; }
        public bool? IsSuperUser { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
