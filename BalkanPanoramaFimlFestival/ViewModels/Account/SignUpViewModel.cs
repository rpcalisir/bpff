using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFilmFestival.ViewModels.Account
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage ="İsim alanı boş bırakılamaz")]
        [Display(Name = "Kullanıcı Adı")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Soyadı alanı boş bırakılamaz")]
        [Display(Name = "Soyadı")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz")]
        [Phone]
        [Display(Name = "Telefon Numarası")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Ülke alanı boş bırakılamaz")]
        [Display(Name = "Ülke")]
        public required string Country { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare(nameof(Password), ErrorMessage = "Şifre aynı değil")]
        public required string ConfirmPassword { get; set; }
    }
}
