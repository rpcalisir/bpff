using Microsoft.EntityFrameworkCore;

namespace BalkanPanoramaFimlFestival.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ContactForm> ContactForms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactForm>().ToTable("contactforms");
            base.OnModelCreating(modelBuilder);
        }
    }
}
