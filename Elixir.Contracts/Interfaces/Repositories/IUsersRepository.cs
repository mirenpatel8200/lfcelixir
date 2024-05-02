using System;
using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IUsersRepository : IRepository<BookUser>
    {
        void UpdateUser(BookUser entity, bool isNewUser = false);
        IEnumerable<BookUser> GetAllUsers(int? limit, UsersSortOrder sortField, SortDirection sortDirections);
        IEnumerable<BookUser> GetUsers(int limit, UsersSortOrder sortField, SortDirection sortDirections);
        IEnumerable<BookUser> GetMembersDirectory();
        IEnumerable<BookUser> GetFoundingMembers();
        //bool IsUserInRole(BookUser user, String role);
        IEnumerable<BookUser> GetByEmail(string email);
        BookUser GetById(int id);
        BookUser GetByIdHashCode(string name);
    }
}
