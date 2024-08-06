using Microsoft.EntityFrameworkCore;
using WkHtmlToPdfDotNet.Contracts;
using WkHtmlToPdfDotNet;
using BalkanPanoramaFilmFestival.Models;
using BalkanPanoramaFilmFestival.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using BalkanPanoramaFilmFestival.Extensions;
using Microsoft.Extensions.Options;
using BalkanPanoramaFilmFestival.Models.OptionsModels;
using BalkanPanoramaFilmFestival.Services; // Add the appropriate namespace

namespace BalkanPanoramaFilmFestival
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

            // If IEmailService exists in any class, give an instance of EmailService to there.
            // Scoped means, after request returns the response, EmailService instance will be deleted from memory,
            // each EmailService instance will be created with each request.
            builder.Services.AddScoped<IEmailService, EmailService>();


            // Use extension method for AddIdentity and Configure
            builder.Services.AddIdentityWithExtension();

            // If IOptions<EmailSettings> is seen in any class's constructor,
            // a new instance of EmailSettings will be created and it's properties will be filled
            // with data that comes from "EmailSettings" on appsettings.json
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // To prevent users to go "film contest application" page without siging in,
                // we define a login path here to redirect them to signin page.
                // So, if User is not Authorized, direct them to login page.
                options.LoginPath = new PathString("/Home/SignIn");

                // Makes it possible to run this action and remove the cookies after sign out
                options.LogoutPath = new PathString("/SignedInUser/LogOut");

                options.AccessDeniedPath = new PathString("/SignedInUser/AccessDenied");

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
