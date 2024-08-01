using BalkanPanoramaFimlFestival.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BalkanPanoramaFimlFestival.Controllers
{
    public class SignedInUserController : Controller
    {
        private readonly SignInManager<RegisteredUser> _signInManager;

        public SignedInUserController(SignInManager<RegisteredUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
