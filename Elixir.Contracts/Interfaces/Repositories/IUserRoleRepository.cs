using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        IEnumerable<UserRole> GetUserRoles(int userId);
        UserRole GetUserRole(int userId, int roleId);
    }
}
