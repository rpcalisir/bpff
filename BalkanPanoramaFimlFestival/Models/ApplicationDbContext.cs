using BalkanPanoramaFilmFestival.Models.Account;
using BalkanPanoramaFilmFestival.Models.CompetitionApplication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BalkanPanoramaFilmFestival.Areas.Admin.ViewModels;
using BalkanPanoramaFilmFestival.Areas.Admin.Models;

namespace BalkanPanoramaFilmFestival.Models
{
    public class ApplicationDbContext : IdentityDbContext<RegisteredUser, RegisteredUserRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<CompetitionApplicationUser> CompetitionApplications { get; set; }
        //public new DbSet<RegisteredUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompetitionApplicationUser>().ToTable("CompetitionApplications");

            //modelBuilder.Entity<ContactForm>().ToTable("contactforms");
            //modelBuilder.Entity<RegisteredUser>(entity =>
            //{
            //    entity.ToTable("users");
            //});

            modelBuilder.Entity<RegisteredUser>(entity =>
            {
                entity.ToTable("AspNetUsers"); // Table name in your database
                entity.Property(e => e.Id).HasColumnName("Id"); // Column names should match
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}