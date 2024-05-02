using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;
using Microsoft.AspNet.Identity;

namespace Elixir.DataAccess.Stores
{
    public class UserRoleStore : IUserRoleStore<BookUser, int>
    {
        private IUsersRepository _usersRepository;
        private IUserRolesRepository _rolesRepository;
        private IUserRoleRepository _userRoleRepository;

        public UserRoleStore(
            IUsersRepository usersRepository,
            IUserRolesRepository rolesRepository,
            IUserRoleRepository userRoleRepository)
        {
            _usersRepository = usersRepository;
            _rolesRepository = rolesRepository;
            _userRoleRepository = userRoleRepository;
        }

        public void Dispose()
        {

        }

        public Task CreateAsync(BookUser user)
        {
            _usersRepository.Insert(user);

            return Task.FromResult(0);
        }

        public Task UpdateAsync(BookUser user)
        {
            _usersRepository.UpdateUser(user);

            return Task.FromResult(0);
        }

        public Task DeleteAsync(BookUser user)
        {
            _usersRepository.Delete(user.Id);

            return Task.FromResult(0);
        }

        public Task<BookUser> FindByIdAsync(int userId)
        {
            var bookUser = _usersRepository.GetById(userId);
            return Task.FromResult(bookUser);
        }

        public Task<BookUser> FindByNameAsync(string userName)
        {
            // TODO: check if it is unique.
            BookUser bookUser = _usersRepository.GetAll().FirstOrDefault(x => x.UserName == userName);

            return Task.FromResult(bookUser);
        }

        public Task AddToRoleAsync(BookUser user, string roleName)
        {
            //BookUserRole userRole = _rolesRepository.GetAll().FirstOrDefault(x => x.Name.Equals(roleName));
            //BookUser dbUser = _usersRepository.GetAll().FirstOrDefault(x => x.Id == user.Id);

            //if (userRole == null) 
            //    throw new NullReferenceException("Role with specified name does not exist.");

            //if(dbUser == null) 
            //    throw new NullReferenceException("User with specified Id does not exist in database.");

            //dbUser.BookUserRole = userRole;

            //_usersRepository.Update(dbUser);

            //return Task.FromResult(0);
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(BookUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(BookUser user)
        {
            //IList<String> roles = _rolesRepository.
            //    GetAll().
            //    Where(x => user.BookUserRole != null && x.Id == user.BookUserRole.Id).
            //    Select(x => x.Name).
            //    ToList();

            //return Task.FromResult(roles);

            IList<string> userRoles = _userRoleRepository.GetUserRoles(user.Id).Select(x => x.Role.RoleName).ToList();
            return Task.FromResult(userRoles);
        }

        public Task<bool> IsInRoleAsync(BookUser user, string roleName)
        {
            //return Task.FromResult(_usersRepository.IsUserInRole(user, ro1leName));
            var userRole = _userRoleRepository.GetUserRole(user.Id, (int)((Roles)Enum.Parse(typeof(Roles), roleName)));
            return Task.FromResult(userRole != null);
        }
    }
}
