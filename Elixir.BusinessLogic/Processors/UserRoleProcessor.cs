using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.BusinessLogic.Processors
{
    public class UserRoleProcessor : IUserRoleProcessor
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleProcessor(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public void CreateUserRole(UserRole entity)
        {
            _userRoleRepository.Insert(entity);
        }

        public void UpdateUserRole(UserRole entity)
        {
            _userRoleRepository.Update(entity);
        }

        public IEnumerable<UserRole> GetUserRoles(int userId)
        {
            return _userRoleRepository.GetUserRoles(userId);
        }

        public UserRole GetUserRole(int userId, int roleId)
        {
            return _userRoleRepository.GetUserRole(userId, roleId);
        }
    }
}
