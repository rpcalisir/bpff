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

namespace BalkanPanoramaFimlFestival.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<RegisteredUser> _userManager;
        private readonly SignInManager<RegisteredUser> _signInManager;
        private readonly ApplicationSettings _appSettings;

        public HomeController(UserManager<RegisteredUser> userManager,
            SignInManager<RegisteredUser> signInManager,
            IOptions<ApplicationSettings> appSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
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
            return View();
        }
    }
}
