using Microsoft.AspNetCore.Identity;

namespace BalkanPanoramaFimlFestival.Localization
{
    public class IdentityErrorDescriberLocalization: IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new() { Code = "DuplicateUserName", Description = $"Bu {userName} daha önce kayıt olmuştur" };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new() { Code = "DuplicateUserName", Description = $"Bu {email} daha önce kayıt olmuştur" };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new() { Code = "PasswordTooShort", Description = $"Şifre en az 6 karakter olmalıdır" };
        }
    }
}
