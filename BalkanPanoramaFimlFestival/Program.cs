using Microsoft.EntityFrameworkCore;
using WkHtmlToPdfDotNet.Contracts;
using WkHtmlToPdfDotNet;
using BalkanPanoramaFimlFestival.Models;
using BalkanPanoramaFimlFestival.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using BalkanPanoramaFimlFestival.Extensions;
using Microsoft.Extensions.Options; // Add the appropriate namespace

namespace BalkanPanoramaFimlFestival
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(ServerVersion.AutoDetect(connectionString))));

            // Register the AppUrl configuration
            builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

            // Use extension method for AddIdentity and Configure
            builder.Services.AddIdentityWithExtension();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // To prevent users to go "film contest application" page without siging in,
                // we define a login path here to redirect them to signin page.
                // So, if User is not Authorized, direct them to login page.
                options.LoginPath = new PathString("/Home/SignIn");

                options.Cookie = new CookieBuilder
                {
                    Name = "bpffRegisteredUserCookie",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    SecurePolicy = CookieSecurePolicy.Always,
                };

                options.ExpireTimeSpan = TimeSpan.FromDays(60);
                options.SlidingExpiration = true; // If true, whenever user enters the website, timespan of cookie is refreshed.
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            // Ensure authentication and authorization middleware is used
            app.UseAuthentication();
            app.UseAuthorization();

            // Add the custom route for the Admin panel
            app.MapControllerRoute(

                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
