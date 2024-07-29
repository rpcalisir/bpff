using Microsoft.EntityFrameworkCore;
using WkHtmlToPdfDotNet.Contracts;
using WkHtmlToPdfDotNet;
using BalkanPanoramaFimlFestival.Models;
using BalkanPanoramaFimlFestival.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies; // Add the appropriate namespace

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

            // Configure FluentEmail with SMTP
            builder.Services
                .AddFluentEmail("bearzalk@gmail.com")
                .AddSmtpSender(new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("bearzalk@gmail.com", "hhml qmtd ymjo ndyg"),
                    EnableSsl = true,
                });

            // Register the IEmailSender service
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            //builder.Services.AddIdentity<RegisteredUser, IdentityRole>()
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();

            // Configure Identity
            builder.Services.AddIdentity<RegisteredUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //// Configure Identity
            //builder.Services.AddIdentity<RegisteredUser, IdentityRole>(options =>
            //{
            //    options.SignIn.RequireConfirmedAccount = true; // Requires email confirmation
            //})
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            //.AddDefaultTokenProviders();

            // Configure cookie authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.ExpireTimeSpan = TimeSpan.FromDays(30); // Set the cookie to expire in 30 days
                    options.SlidingExpiration = true;
                });

            // Register the AppUrl configuration
            builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

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

            // Custom middleware to check if user is authenticated and redirect accordingly
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;

                if (!path.StartsWithSegments("/Account") &&
                    context.User.Identity != null &&
                    context.User.Identity.IsAuthenticated &&
                    !path.StartsWithSegments("/Home"))
                {
                    context.Response.Redirect("/Home/Index");
                }
                else if (path.StartsWithSegments("/Home") &&
                         (context.User.Identity == null ||
                         !context.User.Identity.IsAuthenticated))
                {
                    context.Response.Redirect("/Account/Register");
                }
                else
                {
                    await next.Invoke();
                }
            });

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
