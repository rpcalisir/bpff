using BalkanPanoramaFilmFestival.Extensions;
using BalkanPanoramaFilmFestival.Models;
using BalkanPanoramaFilmFestival.Models.Account;
using BalkanPanoramaFilmFestival.Models.CompetitionApplication;
using BalkanPanoramaFilmFestival.Services;
using BalkanPanoramaFilmFestival.ViewModels.Account;
using BalkanPanoramaFilmFestival.ViewModels.CompetitionApplication;
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
        private readonly ApplicationDbContext _context;
        private readonly ICountryService _countryService;

        public SignedInUserController(SignInManager<RegisteredUser> signInManager,
            UserManager<RegisteredUser> userManager,
            ApplicationDbContext context,
            ICountryService countryService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _countryService = countryService;
        }

        public IActionResult CompetitionApplication()
        {
            //ViewBag.Countries = _countryService.GetAllCountries();

            var model = new CompetitionApplicationUserViewModel
            {
                CompetitionCategory = string.Empty,
                ProductionYear = string.Empty,
                MovieName = string.Empty,
                DirectorName = string.Empty,
                SelectedCountries = new List<string>(),
                AllCountries = _countryService.GetAllCountries() // Fetch the country list
            };

            return View(model); // viewmodel data is being passed to cshtml here, when page is first being displayed.
        }

        [HttpPost]
        public async Task<IActionResult> CompetitionApplication(CompetitionApplicationUserViewModel model)
        {
            if (!model.CompetitionCategory.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one competition category must be selected.");

            }

            if (!model.SelectedCountries.Any())
            {
                ModelState.AddModelError(string.Empty, "At least one country must be selected.");
            }

            if (model.SelectedCountries.Count > 3)
            {
                ModelState.AddModelError(string.Empty, "Max 3 countries can be selected.");
            }

            //In case of signup form data is not valid, return the view without deleting the form data
            if (!ModelState.IsValid)
            {
                //ViewBag.Countries = _countryService.GetAllCountries();
                //return View(model); // Return the view with validation errors

                model.AllCountries = _countryService.GetAllCountries(); // Re-fetch the country list
                return View(model);
            }

            var signedInUser = await _userManager.FindByNameAsync(User!.Identity!.Name!);

            if (signedInUser!.Email != null)
            {
                var user = new CompetitionApplicationUser
                {
                    CompetitionCategory = model.CompetitionCategoryDescription, // Comes from the page form
                    ProductionYear = model.ProductionYear, // Comes from the page form
                    Applicant = $"{signedInUser.FirstName} {signedInUser.LastName}",
                    ApplicantMail = signedInUser.Email,
                    ApplicantCountry = signedInUser.Country,
                    MovieName = model.MovieName, // Comes from the page form
                    DirectorName = model.DirectorName, // Comes from the page form
                    SelectedCountries = string.Join(", ", model.SelectedCountries) // Store as a comma-separated string
                };

                // Save the form data to the database
                _context.CompetitionApplications.Add(user);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    TempData["SuccessMessage"] = "Your application has been submitted successfully.";
                    return RedirectToAction(nameof(CompetitionApplication)); // Redirect to prevent resubmission on refresh
                }
            }

            ModelState.AddModelError(string.Empty, "An error occurred while processing your application");

            return View(model);
        }

        // In case a user wants to go to this page, 
        // it will be directed to LoginPath page
        // which is configured on ConfigureApplicationCookie in Program.cs
        public async Task<IActionResult> Profile()
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

        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.message = "You are not authorized to access this page. Contact the administrator for authorization.";

            return View();
        }
    }
}
