using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFilmFestival.ViewModels.Account
{
    public class PasswordChangeViewModel
    {
        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [DataType(DataType.Password)]
        [Display(Name = "Eski şifre")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public required string PasswordOld { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public required string PasswordNew { get; set; }

        [Required(ErrorMessage = "Yeni şifre alanı boş bırakılamaz")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        [Compare(nameof(PasswordNew), ErrorMessage = "Yeni şifre aynı değil")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public required string ConfirmPasswordNew { get; set; }
    }
}
