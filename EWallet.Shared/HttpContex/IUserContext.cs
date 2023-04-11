using System.Security.Claims;

namespace EWallet.Utility.HttpContex
{
    public interface IUserContext
    {
        ClaimsPrincipal User { get; }
    }
}
