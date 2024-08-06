using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFilmFestival.ViewModels.Account
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Email cannot be empty!")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email format is wrong!")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password cannot be empty!")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }
        
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }
}
