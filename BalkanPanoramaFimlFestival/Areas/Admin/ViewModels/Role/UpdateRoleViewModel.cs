using System.ComponentModel.DataAnnotations;

namespace BalkanPanoramaFilmFestival.Areas.Admin.ViewModels.Role
{
    public class UpdateRoleViewModel
    {
        public required string Id { get; set; }

        [Required(ErrorMessage = "Role name can't be empty")]
        [Display(Name = "Role Name")]
        public required string RoleName { get; set; }

    }
}
