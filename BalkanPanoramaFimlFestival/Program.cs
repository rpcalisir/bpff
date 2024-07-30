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
                    //options.LogoutPath = "/Account/Logout";
                    //options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                    options.SlidingExpiration = true;
                    options.Cookie.Name = "AspNetCore.Cookies"; // Ensure this matches the cookie name used in your app
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Adjust based on your environment
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

            // Custom middleware to handle redirection based on authentication status
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path;

                // Check if the request is for the Logout action, 
                // thus, Logout method in AccountController can be executed.
                if (path.StartsWithSegments("/Account/Logout"))
                {
                    await next.Invoke();
                    return;
                }

                // Check if the user identity is not null
                if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
                {
                    if (!path.StartsWithSegments("/Home"))
                    {
                        context.Response.Redirect("/Home/Index");
                        return;
                    }
                }
                else
                {
                    if (path.StartsWithSegments("/Home"))
                    {
                        context.Response.Redirect("/Account/Register");
                        return;
                    }
                }

                await next();
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
