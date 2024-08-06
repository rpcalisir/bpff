namespace BalkanPanoramaFilmFestival.Areas.Admin.ViewModels.Role
{
    public class AssignRoleToUserViewModel
    {
        public required string Id { get; set; }
        public required string RoleName { get; set; }
        public bool Exist { get; set; }
    }
}
