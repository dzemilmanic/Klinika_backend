using Microsoft.AspNetCore.Identity;

namespace Klinika_backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
