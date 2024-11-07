using Microsoft.AspNetCore.Identity;

namespace Klinika_backend.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
