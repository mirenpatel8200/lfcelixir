using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces
{
    public interface IUsersProcessor
    {
        //IEnumerable<BookUser> GetAllUsers();
        IEnumerable<BookUser> GetAllUsers(int? limit, UsersSortOrder sortOrder, SortDirection sortDirection);
        int CreateUser(BookUser model);
        BookUser GetUserById(int id);
        BookUser GetByIdHashCode(string name);
        void UpdateUser(BookUser model);
        IEnumerable<BookUser> GetUsers(int limit, UsersSortOrder sortField, SortDirection sortDirections);
        IEnumerable<BookUser> GetMembersDirectory();
        IEnumerable<BookUser> GetFoundingMembers();
        void UpdateUserLoginTime(int id);
        BookUser GetUserByEmail(string email);
        void DeleteUser(int id);
    }
}
