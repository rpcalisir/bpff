using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFimlFestival.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        [Compare(nameof(Password), ErrorMessage = "Şifre aynı değil")]
        public required string ConfirmPassword { get; set; }
    }
}
