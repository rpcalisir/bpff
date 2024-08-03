using BalkanPanoramaFilmFestival.Areas.Admin.Models;
using BalkanPanoramaFilmFestival.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BalkanPanoramaFilmFestival.Areas.Admin.Controllers
{
    [Area("Admin")] //Specifies that when admin is in the url, it should look for this HomeController
    public class HomeController : Controller
    {
        private readonly UserManager<RegisteredUser> _userManager;

        public HomeController(UserManager<RegisteredUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var userList = await _userManager.Users.ToListAsync();

            var adminUserViewModelList = userList.Select(x => new UserViewModel()
                {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            }).ToList();

            return View(adminUserViewModelList);
        }

    }
}
