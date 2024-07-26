using Microsoft.AspNetCore.Identity;

namespace BalkanPanoramaFimlFestival.Models.Account
{
    public class RegisteredUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
    //public class RegisteredUser: IdentityUser
    //{
    //    public string Id { get; set; } = Guid.NewGuid().ToString();
    //    public required string FirstName { get; set; }
    //    public required string LastName { get; set; }
    //    public required string Email { get; set; }
    //    public required string PhoneNumber { get; set; }
    //    public required string Country { get; set; }
    //    public required string Password{ get; set; }
    //    public required string PasswordHash { get; set; }
    //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //}
    //}
//}
