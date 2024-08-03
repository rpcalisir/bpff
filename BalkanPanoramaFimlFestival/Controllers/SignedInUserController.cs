using BalkanPanoramaFilmFestival.Models.Account;
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

        public SignedInUserController(SignInManager<RegisteredUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // In case a user wants to go to this page, 
        // it will be directed to LoginPath page which is configured on ConfigureApplicationCookie in Program.cs
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
