using BalkanPanoramaFimlFestival.Models.Account;
using BalkanPanoramaFimlFestival.Models;
using Microsoft.AspNetCore.Identity;
using BalkanPanoramaFimlFestival.CustomValidations;
using BalkanPanoramaFimlFestival.Localization;

namespace BalkanPanoramaFimlFestival.Extensions
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

            services.AddIdentity<RegisteredUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                //options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                //options.SignIn.RequireConfirmedAccount = true;

                //Lockout
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddUserValidator<UserValidator>() // Use custom validator
            .AddErrorDescriber<IdentityErrorDescriberLocalization>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        }
    }
}
