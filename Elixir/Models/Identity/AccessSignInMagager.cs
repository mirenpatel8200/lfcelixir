using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Elixir.Models.Identity
{
    public class AccessSignInMagager : SignInManager<BookUser, int>
    {
        public AccessSignInMagager(AccessUserManager userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
        {
        }

        public static AccessSignInMagager Create(IdentityFactoryOptions<AccessSignInMagager> identityFactoryOptions,
            IOwinContext owinContext)
        {
            return new AccessSignInMagager(owinContext.GetUserManager<AccessUserManager>(), owinContext.Authentication);
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(BookUser user)
        {
            return user.GenerateUserIdentityAsync(UserManager);
        }
    }
}