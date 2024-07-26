using Microsoft.EntityFrameworkCore;
using WkHtmlToPdfDotNet.Contracts;
using WkHtmlToPdfDotNet;
using BalkanPanoramaFimlFestival.Models;
using BalkanPanoramaFimlFestival.Models.Account;
using Microsoft.AspNetCore.Identity; // Add the appropriate namespace

namespace BalkanPanoramaFimlFestival
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add DbContext with MySQL
            //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            //builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(ServerVersion.AutoDetect(connectionString))));


            // Register the WkHtmlToPdfDotNet converter
            builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));

            //builder.Services.AddIdentity<RegisteredUser, IdentityRole>()
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();

            // Configure Identity
            //builder.Services.AddIdentity<RegisteredUser, IdentityRole>(options =>
            //{
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireDigit = true;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireLowercase = false;
            //})
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();

            // Configure Identity
            builder.Services.AddIdentity<RegisteredUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Add the custom route for the Admin panel
            app.MapControllerRoute(
                
                name: "admin",
                pattern: "admin/{action=AdminPanel}/{id?}",
                defaults: new { controller = "admin" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Register}/{id?}");

            app.Run();
        }
    }
}
