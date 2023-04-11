using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EWallet.Utility.JwtHandler
{
    public interface IJwtTokenMethod
    {
        Task<string> GenerateJwtToken(IdentityUser identityUser);
    }
}
