using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFilmFestival.ViewModels.Account
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage ="First Name cannot be empty!")]
        [Display(Name = "Name")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name cannot be empty!")]
        [Display(Name = "Lastname")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email cannot be empty!")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email format is wrong!")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone Number cannot be empty!")]
        [Phone]
        [Display(Name = "Phone Number")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Country cannot be empty!")]
        [Display(Name = "Country")]
        public required string Country { get; set; }

        [Required(ErrorMessage = "Password cannot be empty!")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Password Confirm cannot be empty!")]
        [DataType(DataType.Password)]
        [Display(Name = "Password Confirm")]
        [Compare(nameof(Password), ErrorMessage = "Passwords does not match!")]
        public required string ConfirmPassword { get; set; }
    }
}
