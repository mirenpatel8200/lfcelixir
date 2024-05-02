using Elixir.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Practices.Unity;
using Owin;

[assembly: OwinStartup(typeof(Elixir.App_Start.Startup))]
namespace Elixir.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = new UnityContainer();

            app.CreatePerOwinContext<AccessUserManager>(AccessUserManager.Create);
            app.CreatePerOwinContext<AccessSignInMagager>(AccessSignInMagager.Create);

            app.CreatePerOwinContext<AccessRoleManager>(AccessRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Page/Login"),
            });
        }
    }
}