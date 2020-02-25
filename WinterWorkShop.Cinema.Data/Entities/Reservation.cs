using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("Reservation")]
    public class Reservation
    {
        public int id { get; set; }
        //public bool reservation { get; set; }

        [Column("projection_id")]
        public Guid projectionId { get; set; }

        [Column("seat_id")]
        public Guid seatId { get; set; }

        public virtual Projection Projection { get; set; }

        public virtual Seat Seat { get; set; }
    }
}
