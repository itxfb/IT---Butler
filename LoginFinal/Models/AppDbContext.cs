using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginFinal.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Education> Education { get; set; }


        //////JuctionTable
        public DbSet<Message> Messages { get; set; }

        ////public DbSet<Orders> Orders { get; set; }
        //////JunctionTable

        public DbSet<Logging> Logging { get; set; }
        public DbSet<Skills> Skills { get; set; }

        public DbSet<Tags> Tags { get; set; }

        public DbSet<Refferals> Refferals { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Education>()
             .HasOne<User>(e => e.Users)
             .WithMany(d => d.Education)
             .HasForeignKey(e => e.UserId)
             .IsRequired(false);

            modelBuilder.Entity<Tags>()
            .HasOne<User>(e => e.Users)
            .WithMany(d => d.Tags)
            .HasForeignKey(e => e.UserId)
            .IsRequired(false);

            modelBuilder.Entity<Skills>()
            .HasOne<User>(e => e.Users)
            .WithMany(d => d.Skills)
            .HasForeignKey(e => e.UserId)
            .IsRequired(false);

            modelBuilder.Entity<Message>()
            .HasOne<User>(e => e.Users)
            .WithMany(d => d.Messages)
            .HasForeignKey(e => e.SenderId)
            .IsRequired(false);


            modelBuilder.Entity<Message>()
            .HasOne<User>(e => e.RecieverEnd)
            .WithMany(d => d.RecieverEnd)
            .HasForeignKey(e => e.RecieverId)
            .IsRequired(false);


            modelBuilder.Entity<Logging>()
            .HasOne<User>(e => e.Users)
            .WithMany(d => d.Logging)
            .HasForeignKey(e => e.UserId)
            .IsRequired(false);


            modelBuilder.Entity<Refferals>()
           .HasOne<User>(e => e.RefferedUser)
           .WithMany(d => d.Refferals)
           .HasForeignKey(e => e.RefferedId)
           .IsRequired(false);

            //  modelBuilder.Entity<Tags>()
            //  .HasMany(c => c.Instructors).WithMany(i => i.Courses)
            //  .Map(t => t.MapLeftKey("CourseID")
            //.MapRightKey("InstructorID")
            //.ToTable("CourseInstructor"));
        }



    }

}
