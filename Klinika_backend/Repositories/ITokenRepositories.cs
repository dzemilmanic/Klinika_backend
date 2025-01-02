using Klinika_backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Klinika_backend.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(ApplicationUser user, List<string> roles);
    }
}
