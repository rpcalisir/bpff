using BalkanPanoramaFimlFestival.Models.Account;
using BalkanPanoramaFimlFestival.Models;
using Microsoft.AspNetCore.Identity;
using BalkanPanoramaFimlFestival.CustomValidations;
using BalkanPanoramaFimlFestival.Localization;

namespace BalkanPanoramaFimlFestival.Extensions
{
    public static class ProgramExtensions
    {
        public static void AddIdentityWithExtension(this IServiceCollection services)
        {
            services.AddIdentity<RegisteredUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                //options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                //options.SignIn.RequireConfirmedAccount = true;
            })
            .AddUserValidator<UserValidator>() // Use custom validator
            .AddErrorDescriber<IdentityErrorDescriberLocalization>() 
            .AddEntityFrameworkStores<ApplicationDbContext>();
        }
    }
}
