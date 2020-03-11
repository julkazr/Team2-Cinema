using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinterWorkShop.Cinema.Data.Entities
{
    [Table("Reservation")]
    public class Reservation
    {
        public int id { get; set; }

        [Column("projection_id")]
        public Guid projectionId { get; set; }

        [Column("seat_id")]
        public Guid seatId { get; set; }

        [Column("user_id")]
        public Guid userId { get; set; }

        public virtual Projection Projection { get; set; }

        public virtual Seat Seat { get; set; }

        public virtual User User { get; set; }

    }
}
