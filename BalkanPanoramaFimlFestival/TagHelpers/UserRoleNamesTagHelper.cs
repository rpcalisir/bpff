using BalkanPanoramaFilmFestival.Models.Account;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace BalkanPanoramaFilmFestival.TagHelpers
{
    public class UserRoleNamesTagHelper:TagHelper
    {
        public required string UserId { get; set; }
        private readonly UserManager<RegisteredUser> _userManager;

        public UserRoleNamesTagHelper(UserManager<RegisteredUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            var userRoles = await _userManager.GetRolesAsync(user!);

            var stringBuilder = new StringBuilder();

            userRoles.ToList().ForEach(userRole =>
                stringBuilder.Append(@$"<span class='badge bg-secondary mx-1'>{userRole.ToLower()}</span>")
            );

            output.Content.SetHtmlContent(stringBuilder.ToString());
        }

    }
}
