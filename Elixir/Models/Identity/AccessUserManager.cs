using Elixir.App_Start;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.DataAccess.Stores;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;

namespace Elixir.Models.Identity
{
    public class AccessUserManager : UserManager<BookUser, int>
    {
        public AccessUserManager(IUserStore<BookUser, int> store) : base(store)
        {
        }

        public static AccessUserManager Create()
        {
            IUsersRepository usersRepository = UnityConfig.GetConfiguredContainer().Resolve<IUsersRepository>();
            IUserRolesRepository rolesRepository = UnityConfig.GetConfiguredContainer().Resolve<IUserRolesRepository>();
            IUserRoleRepository userRoleRepository = UnityConfig.GetConfiguredContainer().Resolve<IUserRoleRepository>();

            return new AccessUserManager(new UserRoleStore(usersRepository, rolesRepository, userRoleRepository));
        }
    }
}