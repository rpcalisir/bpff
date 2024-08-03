using BalkanPanoramaFilmFestival.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace BalkanPanoramaFilmFestival.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<RegisteredUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<RegisteredUser> manager, RegisteredUser user, string? password)
        {
            var errors = new List<IdentityError>();

            if (password!.Length < 6) 
            {
                    errors.Add(new IdentityError() { Code = "PasswordLengthIsNotSufficient", Description = "Şifre uzunluğu en az 6 karakter olmalıdır."});
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
