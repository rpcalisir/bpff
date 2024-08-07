using BalkanPanoramaFilmFestival.Models.Account;
using BalkanPanoramaFilmFestival.Models;
using Microsoft.AspNetCore.Identity;
using BalkanPanoramaFilmFestival.CustomValidations;
using BalkanPanoramaFilmFestival.Localization;
using BalkanPanoramaFilmFestival.Areas.Admin.Models;

namespace BalkanPanoramaFilmFestival.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExtension(this IServiceCollection services)
        {
            // ResetPassword token lifespan configuration
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(2);
            });

            // If user data is changed in another browser,
            // security stamp automatically signs out and redirects user to signin page
            // on the browser which cookies stored with old information.
            // SecurityStamp is not called by itself, it is called explicitly like here.
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(30);
            });

            services.AddIdentity<RegisteredUser, RegisteredUserRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                //options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                //options.SignIn.RequireConfirmedAccount = true;

                // Lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;

                // Confirmation Email Sign In
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddUserValidator<UserValidator>() // Use custom validator
            .AddErrorDescriber<IdentityErrorDescriberLocalization>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();  
        }
    }
}
