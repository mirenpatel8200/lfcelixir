using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces
{
    public interface IUserRoleProcessor
    {
        void CreateUserRole(UserRole entity);
        void UpdateUserRole(UserRole entity);
        IEnumerable<UserRole> GetUserRoles(int userId);
        UserRole GetUserRole(int userId, int roleId);
    }
}
