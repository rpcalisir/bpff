using BalkanPanoramaFilmFestival.Extensions;
using BalkanPanoramaFilmFestival.Models.Account;
using BalkanPanoramaFilmFestival.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BalkanPanoramaFilmFestival.Controllers
{
    // Only SignedIn User can use this Controller and its methods
    [Authorize]
    public class SignedInUserController : Controller
    {
        private readonly SignInManager<RegisteredUser> _signInManager;
        private readonly UserManager<RegisteredUser> _userManager;

        public SignedInUserController(SignInManager<RegisteredUser> signInManager, 
            UserManager<RegisteredUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // In case a user wants to go to this page, 
        // it will be directed to LoginPath page
        // which is configured on ConfigureApplicationCookie in Program.cs
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.Name == null)
            {
                return RedirectToAction("SignIn", "Home"); // Redirect to signIn if user identity is null
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Home"); // Redirect to signIn if user is not found
            }

            var signedInUserViewModel = new SignedInUserViewModel
            {
                FirstName = currentUser.FirstName ?? string.Empty,
                LastName = currentUser.LastName ?? string.Empty,
                Email = currentUser.Email ?? string.Empty,
                PhoneNumber = currentUser.PhoneNumber ?? string.Empty
            };

            return View(signedInUserViewModel);
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel model)
        {
            //In case of password change form data is not valid,
            //return the view without deleting the form data
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Because of this controller has [Authorize] on it, 
            // User cannot be null, that's why it is safe to put ! next to it.
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            bool checkOldPassword = await _userManager.CheckPasswordAsync(currentUser!, model.PasswordOld);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski şifre yanlış");
                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser!, model.PasswordOld, model.PasswordNew);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors.Select(d => d.Description).ToList());
                return View();
            }

            // Update the security stamp after password change,
            // so sessions in other browsers can become signed out.
            await _userManager.UpdateSecurityStampAsync(currentUser!);

            // Signout and signin the user after the password change,
            // to update the cookie.
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser!, model.PasswordNew, true, false);

            TempData["SuccessMessage"] = "Şifre başarı ile değiştirildi";

            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
