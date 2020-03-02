using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Data
{
    public class CinemaContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Projection> Projections { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Auditorium> Auditoriums { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TagMovie> TagMovies { get; set; }
        public DbSet<Reservation> Reservations { get; set; }



        public CinemaContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /// <summary>
            /// Seat -> Auditorium relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Seat>()
                .HasOne(x => x.Auditorium)
                .WithMany(x => x.Seats)
                .HasForeignKey(x => x.AuditoriumId)
                .IsRequired();

            /// <summary>
            /// Auditorium -> Seat relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Auditorium>()
                .HasMany(x => x.Seats)
                .WithOne(x => x.Auditorium)
                .IsRequired();


            /// <summary>
            /// Cinema -> Auditorium relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Cinema>()
                .HasMany(x => x.Auditoriums)
                .WithOne(x => x.Cinema)
                .IsRequired();
            
            /// <summary>
            /// Auditorium -> Cinema relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Auditorium>()
                .HasOne(x => x.Cinema)
                .WithMany(x => x.Auditoriums)
                .HasForeignKey(x => x.CinemaId)
                .IsRequired();


            /// <summary>
            /// Auditorium -> Projection relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Auditorium>()               
               .HasMany(x => x.Projections)
               .WithOne(x => x.Auditorium)
               .IsRequired();

            /// <summary>
            /// Projection -> Auditorium relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Projection>()
                .HasOne(x => x.Auditorium)
                .WithMany(x => x.Projections)
                .HasForeignKey(x => x.AuditoriumId)
                .IsRequired();


            /// <summary>
            /// Projection -> Movie relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Projection>()
                .HasOne(x => x.Movie)
                .WithMany(x => x.Projections)
                .HasForeignKey(x => x.MovieId)
                .IsRequired();

            /// <summary>
            /// Movie -> Projection relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Movie>()
                .HasMany(x => x.Projections)
                .WithOne(x => x.Movie)
                .IsRequired();

            modelBuilder.Entity<Reservation>()
                .HasOne(x => x.Projection)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.projectionId)
                .IsRequired();

            modelBuilder.Entity<Projection>()
                .HasMany(x => x.Reservations)
                .WithOne(x => x.Projection)
                .IsRequired();

            modelBuilder.Entity<Reservation>()
                .HasOne(x => x.Seat)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.seatId)
                .IsRequired();

            modelBuilder.Entity<Seat>()
                .HasMany(x => x.Reservations)
                .WithOne(x => x.Seat)
                .IsRequired();

            //******************************************************************

            /// <summary>
            /// Movie -> TagMovie relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Movie>()
               .HasMany(x => x.TagMovies)
               .WithOne(x => x.Movie)
               .IsRequired();

            /// <summary>
            /// TagMovie -> Movie relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<TagMovie>()
                .HasOne(x => x.Movie)
                .WithMany(x => x.TagMovies)
                .HasForeignKey(x => x.MovieId)
                .IsRequired();


            /// <summary>
            /// TagMovie -> Tag relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<TagMovie>()
                .HasOne(x => x.Tag)
                .WithMany(x => x.TagMovies)
                .HasForeignKey(x => x.TagId)
                .IsRequired();

            /// <summary>
            /// Tag -> TagMovie relation
            /// </summary>
            /// <returns></returns>
            modelBuilder.Entity<Tag>()
                .HasMany(x => x.TagMovies)
                .WithOne(x => x.Tag)
                .IsRequired();

            //modelBuilder.Entity<Reservation>()
            //   .HasOne(x => x.Seat)
            //   .WithMany(x => x.Reservations)
            //   .HasForeignKey(x => x.seatId)
            //   .IsRequired();

            //modelBuilder.Entity<Seat>()
            //    .HasMany(x => x.Reservations)
            //    .WithOne(x => x.Seat)
            //    .IsRequired();

            modelBuilder.Entity<Reservation>()
               .HasOne(x => x.User)
               .WithMany(x => x.Reservations)
               .HasForeignKey(x => x.userId)
               .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(x => x.Reservations)
                .WithOne(x => x.User)
                .IsRequired();

        }
    }
}
