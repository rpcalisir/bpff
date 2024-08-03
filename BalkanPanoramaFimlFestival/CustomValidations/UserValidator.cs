using BalkanPanoramaFilmFestival.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace BalkanPanoramaFilmFestival.CustomValidations
{
    public class UserValidator : IUserValidator<RegisteredUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<RegisteredUser> manager, RegisteredUser user)
        {
            var errors = new List<IdentityError>();
            var isNumeric = int.TryParse(user!.FirstName[0].ToString(), out _);

            if (isNumeric)
            {
                errors.Add(new IdentityError() { Code = "UserNameStartsWithDigit", 
                    Description = "Kullanıcı adının ilk karakteri sayısal olamaz" });
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
