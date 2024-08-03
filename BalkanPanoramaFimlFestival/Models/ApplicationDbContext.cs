using BalkanPanoramaFilmFestival.Models.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BalkanPanoramaFilmFestival.Models
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<RegisteredUser>(options)
    {
        public DbSet<ContactForm> ContactForms { get; set; }
        public new DbSet<RegisteredUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactForm>().ToTable("contactforms");
            modelBuilder.Entity<RegisteredUser>(entity =>
            {
                entity.ToTable("users");
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}