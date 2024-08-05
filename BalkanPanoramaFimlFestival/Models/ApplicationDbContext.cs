using BalkanPanoramaFilmFestival.Models.Account;
using BalkanPanoramaFilmFestival.Models.CompetitionApplication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BalkanPanoramaFilmFestival.Areas.Admin.Models;

namespace BalkanPanoramaFilmFestival.Models
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<RegisteredUser>(options)
    {
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
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<BalkanPanoramaFilmFestival.Areas.Admin.Models.CompetitionApplicationUserViewModel> CompetitionApplicationUserViewModel { get; set; } = default!;
    }
}