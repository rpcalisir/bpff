using BalkanPanoramaFilmFestival.Areas.Admin.Models;
using BalkanPanoramaFilmFestival.Areas.Admin.ViewModels.Role;
using BalkanPanoramaFilmFestival.Extensions;
using BalkanPanoramaFilmFestival.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BalkanPanoramaFilmFestival.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")] // Only the users with admin role can access to admin panel
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<RegisteredUser> _userManager;
        private readonly RoleManager<RegisteredUserRole> _roleManager;

        public RolesController(UserManager<RegisteredUser> userManager,
            RoleManager<RegisteredUserRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "developer")]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(role => new RoleViewModel()
            {
                Id = role.Id,
                RoleName = role.Name!
            }).ToListAsync();

            return View(roles);
        }

        [Authorize(Roles = "developer")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [Authorize(Roles = "developer")]
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            var result = await _roleManager.CreateAsync(new RegisteredUserRole() { Name = model.RoleName });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors.Select(d => d.Description).ToList());
                return View();
            }
            TempData["SuccessMessage"] = "Role has been created successfully";

            return RedirectToAction(nameof(RolesController.Index));
        }

        [Authorize(Roles = "developer")]
        public async Task<IActionResult> UpdateRole(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);

            if (roleToUpdate == null)
            {
                throw new Exception("Role to update could not be found!");
            }

            return View(new UpdateRoleViewModel() { Id = roleToUpdate.Id, RoleName = roleToUpdate!.Name! });
        }

        [Authorize(Roles = "developer")]
        [HttpPost]
        public async Task<IActionResult> UpdateRole(UpdateRoleViewModel model)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(model.Id);

            if (roleToUpdate == null)
            {
                throw new Exception("Role to update could not be found!");
            }

            roleToUpdate.Name = model.RoleName;
            await _roleManager.UpdateAsync(roleToUpdate);

            ViewData["SuccessMessage"] = "Role name is updated";

            return View();
        }

        [Authorize(Roles = "developer")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);

            if (roleToDelete == null)
            {
                throw new Exception("Role to delete could not be found!");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(d => d.Description).First());
            }

            TempData["SuccessMessage"] = "Role has been deleted successfully";

            return RedirectToAction(nameof(RolesController.Index));
        }

        [Authorize(Roles = "developer")]
        public async Task<IActionResult> AssignRoleToUser(string userId)
        {
            // Get current user
            var currentUser = await _userManager.FindByIdAsync(userId);
            //var currentUser = await _userManager.FindByEmailAsync("bearzalk@gmail.com");

            //var userIdNew = await _userManager.GetUserIdAsync(currentUser!);

            //var isUserFound = await _userManager.FindByIdAsync(userIdNew);


            // Add userId into ViewBag to be able to send it to view,
            // so it can be possible to use it in AssignRoleToUser post request action method
            ViewBag.userId = userId;
            //ViewBag.userEmail = email;

            // Get all defined role types
            var roles = await _roleManager.Roles.ToListAsync();

            // Get the roles that is assigned to user
            var userRoles = await _userManager.GetRolesAsync(currentUser!);

            var roleViewModelList = new List<AssignRoleToUserViewModel>();

            // In all role types, if the role type is associated with the current user,
            // make the Exist property true for newly created AssignRoleToUserViewModel object
            // and add it into the list
            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    Id = role.Id,
                    RoleName = role.Name!
                };

                if(userRoles.Contains(role.Name!))
                {
                    assignRoleToUserViewModel.Exist = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }

        [Authorize(Roles = "developer")]
        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(List<AssignRoleToUserViewModel> requestList, string userId)
        {
            var userToAssignRoles = (await _userManager.FindByIdAsync(userId))!;

            foreach (var role in requestList)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.RoleName);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.RoleName);
                }
            }

            if (!requestList.Any(r => r.Exist))
            {
                ModelState.AddModelError(string.Empty, "Please select at least one role.");
                // Return the view with the existing data and user ID
                ViewBag.userId = userId;
                return View(requestList);
            }


            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }

    }
}
