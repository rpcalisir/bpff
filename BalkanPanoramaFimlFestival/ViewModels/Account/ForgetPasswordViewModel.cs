using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFimlFestival.ViewModels.Account
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        public required string Email { get; set; }
    }
}
