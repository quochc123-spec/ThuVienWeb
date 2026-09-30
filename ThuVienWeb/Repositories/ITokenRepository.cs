using Microsoft.AspNetCore.Identity;

namespace ThuVienWeb.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
