using BalkanPanoramaFimlFestival.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System;
using WkHtmlToPdfDotNet.Contracts;
using WkHtmlToPdfDotNet;
using BalkanPanoramaFimlFestival.Models.Account;
using BalkanPanoramaFimlFestival.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using BalkanPanoramaFimlFestival.Extensions;

namespace BalkanPanoramaFimlFestival.Controllers
{
    public class HomeController : Controller
    {
        // Used for create/find operations on database table for user
        private readonly UserManager<RegisteredUser> _userManager;

        // Used for sign in, third party authentication and cookie management
        private readonly SignInManager<RegisteredUser> _signInManager;

        // Used for url redirection for development and release
        private readonly ApplicationSettings _appSettings;

        public HomeController(UserManager<RegisteredUser> userManager,
            SignInManager<RegisteredUser> signInManager,
            IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
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
            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var foundUser = await _userManager.FindByEmailAsync(model.Email);

            if (foundUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı!");
                return View();
            }

            // If lockoutOnFailure is true, system will be locked after three unsuccessful login attempts.
            // RememberMe is being handled here with isPersistent
            var signInResult = await _signInManager.PasswordSignInAsync(foundUser, model.Password, model.RememberMe, true); 

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl!);
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giriş yapamazsınız!"
                });
                return View();
            }

            ModelState.AddModelErrorList(new List<string>() { 
                "Email onaylanmadı veya şifre yanlış! ",
                $"(Başarısız giriş sayısı:{await _userManager.GetAccessFailedCountAsync(foundUser)})(MAX:3)" 
            });

            return View();
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterViewModel model)
        {
            //In case of register form data is not valid, return the view without deleting the form data
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
    }
}
