using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFilmFestival.ViewModels.Account
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public required string Password { get; set; }
        
        [Display(Name = "Beni Hatırla?")]
        public bool RememberMe { get; set; }

    }
}
