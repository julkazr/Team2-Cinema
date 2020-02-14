using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
    }
}
