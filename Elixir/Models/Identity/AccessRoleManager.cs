using Elixir.DataAccess.Repositories.MsAccess;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Elixir.Models.Identity
{
    public class AccessRoleManager : RoleManager<BookUserRole, int>
    {
        public AccessRoleManager(IRoleStore<BookUserRole, int> store) : base(store)
        {
        }

        public static AccessRoleManager Create(IdentityFactoryOptions<AccessRoleManager> identityFactoryOptions, IOwinContext owinContext)
        {
            return new AccessRoleManager(new RolesRepository());
        }
    }
}