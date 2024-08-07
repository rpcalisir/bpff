using Microsoft.AspNetCore.Mvc;
using BalkanPanoramaFilmFestival.Models.Account;
using BalkanPanoramaFilmFestival.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using BalkanPanoramaFilmFestival.Extensions;
using BalkanPanoramaFilmFestival.Services;
using System.Text.Encodings.Web;

namespace BalkanPanoramaFilmFestival.Controllers
{
    public class HomeController : Controller
    {
        // Used for create/find operations on database table for user
        private readonly UserManager<RegisteredUser> _userManager;

        // Used for sign in, third party authentication and cookie management
        private readonly SignInManager<RegisteredUser> _signInManager;

        // Used for url redirection for development and release
        private readonly ApplicationSettings _appSettings;

        private readonly IEmailService _emailService;

        public HomeController(UserManager<RegisteredUser> userManager,
            SignInManager<RegisteredUser> signInManager,
            IOptions<ApplicationSettings> appSettings,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string? returnUrl = null)
        {
            //In case of signin form data is not valid, return the view without deleting the form data
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var foundUser = await _userManager.FindByEmailAsync(model.Email);

            if (foundUser == null)
            {
                ModelState.AddModelError(string.Empty, "User cannot be found!");
                return View();
            }

            // Check if the email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(foundUser))
            {
                ModelState.AddModelError(string.Empty, "Email is not confirmed!");
                return View();
            }

            // If lockoutOnFailure is true, system will be locked after three unsuccessful signin attempts.
            // RememberMe is being handled here with isPersistent
            var signInResult = await _signInManager.PasswordSignInAsync(foundUser, model.Password, model.RememberMe, true);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl!);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "Cannot sign in for 3 minutes!"
                });
                return View();
            }

            ModelState.AddModelErrorList(new List<string>() {
                "Password is wrong! ",
                $"(Number of failed sign in attempts:{await _userManager.GetAccessFailedCountAsync(foundUser)})(MAX:3)"
            });

            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            //In case of signup form data is not valid, return the view without deleting the form data
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new RegisteredUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Country = model.Country,
                CreatedAt = DateTime.UtcNow
            };

            // CreateAsync method creates a user and saves into the db table,
            // in case there is an existing user, it returns error message inside of return object
            var identityResult = await _userManager.CreateAsync(user, model.ConfirmPassword);

            if (identityResult.Succeeded)
            {
                // Generate email confirmation token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Create the confirmation URL
                var callbackUrl = Url.Action(
                    nameof(ConfirmEmail),
                    "Home",
                    new { userId = user.Id, code = code },
                    protocol: Request.Scheme);

                // Send the confirmation email
                await _emailService.SendEmailAsync(
                    model.Email,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>.");

                // Set a success message in TempData
                TempData["SuccessMessage"] = "Üyelik işlemi başarı ile gerçekleşmiştir. Giriş için mail adresinizi onaylayınız.";

                //Return current page, with calling Register(Get) method, passing TempData into it,
                //so message can be passed and empty form can be seen.
                return RedirectToAction(nameof(SignUp));
            }

            // This is to display general Errors on Register Page.
            // It stores errors that comes from CreateAsync method return, into the ModelState.
            foreach (IdentityError error in identityResult.Errors)
            {
                //If AddModelError's first parameter is being given empty, it means it's used for a general error.
                ModelState.AddModelError(string.Empty, error.Description);
            }


            // Usage of extension method
            //ModelState.AddModelErrorList(identityResult.Errors.Select(d => d.Description).ToList());

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Email is verified successfully";
                return RedirectToAction("SignIn", "Home");
            }

            TempData["ErrorMessage"] = "Error confirming your email.";
            return RedirectToAction("SignUp", "Home");
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            var hasUser = await _userManager.FindByEmailAsync(model.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Bu email adresine sahip kullanıcı yoktur");

                // Returns a request to Http Get method ForgetPassword,
                // so it returns ForgetPassword.cshtml with empty form.
                // Requests are stateless, thus the data cannot be transfered between requests.
                // If Redirect was used here, error in ModelState would be lost,
                // because data cannot be transfered via ModelState.
                // If Redirect was used here, TempData can be used to move error data.
                return View();
            }

            // Generate reset token and link
            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
            var passwordResetLink = Url.Action("ResetPassword", "Home",
                new { userId = hasUser.Id, Token = passwordResetToken },
                HttpContext.Request.Scheme); // Adds host at the beginning of the url

            // Email Service
            await _emailService.SendResetPasswordMail(passwordResetLink!, hasUser.Email!);

            // If these lines are used, after this message is appeared on the screen and
            // user reloads the screen, the user will keep seeing this message, which is not a desired behavior.
            // Basically, this message will stay on the screen even after page reload.
            //ViewBag.SuccessMessage = "Şifre yenileme linki mail adresine gönderildi";
            //return View();

            TempData["SuccessMessage"] = "Şifre yenileme linki mail adresine gönderildi";
            return RedirectToAction(nameof(ForgetPassword));
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            // TempData can be read only for once, if the page is refreshed then it is lost.
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                throw new Exception("Bir hata meydana geldi");
            }


            var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, model.Password);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifre başarıyla yenilenmiştir";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(d => d.Description).ToList());
            }

            return View();
        }

    }
}
