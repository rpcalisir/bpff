using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFimlFestival.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public required string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Country")]
        public required string Country { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
