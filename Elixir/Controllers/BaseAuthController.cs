using System.Web;
using Elixir.Models.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Elixir.Controllers
{
    public class BaseAuthController : BaseController
    {
        private AccessSignInMagager _signInManager;
        private AccessUserManager _userManager;


        protected AccessSignInMagager AccessSignInManager => _signInManager ?? HttpContext.GetOwinContext().Get<AccessSignInMagager>();

        protected AccessUserManager AccessUserManager => _userManager ?? HttpContext.GetOwinContext().Get<AccessUserManager>();

        protected IAuthenticationManager AuthenticationManager => AccessSignInManager.AuthenticationManager;
    }
}