using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using BalkanPanoramaFimlFestival.ViewModels.Account;
using BalkanPanoramaFimlFestival.Models.Account;
using BalkanPanoramaFimlFestival.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace BalkanPanoramaFimlFestival.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<RegisteredUser> _userManager;
        private readonly SignInManager<RegisteredUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationSettings _appSettings;

        // Primary Constructor
        public AccountController(UserManager<RegisteredUser> userManager, 
            SignInManager<RegisteredUser> signInManager, IEmailSender emailSender,
            IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email already registered.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
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

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Scheme, host: _appSettings.AppUrl);

                    if (callbackUrl != null)
                    {
                        await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        TempData["Message"] = "Registration is successfull. \n A verification email has been sent, check your email please!";
                        return RedirectToAction("Register");
                    }
                    else
                    {
                        // Handle the case where the URL generation fails
                        ModelState.AddModelError(string.Empty, "Error generating confirmation link.");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                TempData["ConfirmationMessage"] = "Confirmation Successful";
            }
            else
            {
                TempData["ConfirmationMessage"] = "Confirmation Failed";
            }

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        //{
        //if (ModelState.IsValid)
        //{
        //    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

        //    if (result.Succeeded)
        //    {
        //        TempData["Message"] = "Login successful!";
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //    }
        //}
        //return View(model);
        //}

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // User exists but email is not confirmed, sign out the user if signed in
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "Your email address is not confirmed. Please check your email for the confirmation link.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Save the user information in cookies if RememberMe is true
                    if (model.RememberMe)
                    {
                        Response.Cookies.Append("UserEmail", model.Email, new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30),
                            IsEssential = true,
                            HttpOnly = true,
                            Secure = true
                        });

                        Response.Cookies.Append("UserPassword", model.Password, new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30),
                            IsEssential = true,
                            HttpOnly = true,
                            Secure = true
                        });
                    }

                    // Redirect to the return URL if provided, or default to Index page
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            // If we got this far, something failed, redisplay the form
            return View(model);
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Register", "Account");
        }
    }
}
