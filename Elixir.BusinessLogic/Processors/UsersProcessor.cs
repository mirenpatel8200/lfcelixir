using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class UsersProcessor : IUsersProcessor
    {
        private IUsersRepository _usersRepository;

        public UsersProcessor(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        //public IEnumerable<BookUser> GetAllUsers()
        //{
        //    return GetAllUsers(UsersSortOrder.Id, SortDirection.Descending, UsersRecordLimit.All);
        //}

        public IEnumerable<BookUser> GetAllUsers(int? limit, UsersSortOrder sortOrder, SortDirection sortDirection)
        {
            return _usersRepository.GetAllUsers(limit, sortOrder, sortDirection);
            //IEnumerable<BookUser> allUsers = _usersRepository.GetAll();

            //if (sortDirection == SortDirection.Ascending)
            //{
            //    switch (sortOrder)
            //    {
            //        case UsersSortOrder.Id:
            //            allUsers = allUsers.OrderBy(x => x.Id).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.FirstName:
            //            allUsers = allUsers.OrderBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.LastName:
            //            allUsers = allUsers.OrderBy(x => x.UserNameLast).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.EmailAddress:
            //            allUsers = allUsers.OrderBy(x => x.EmailAddress).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.Role:
            //            allUsers = allUsers.OrderBy(x => x.Role?.RoleName).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.IsEnabled:
            //            allUsers = allUsers.OrderBy(x => x.IsEnabled).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.LastLogin:
            //            allUsers = allUsers.OrderBy(x => x.LastLogin).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.MemberNumber:
            //            allUsers = allUsers.OrderBy(x => x.MemberNumber).ThenBy(x => x.UserName);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            //    }
            //}
            //else if (sortDirection == SortDirection.Descending)
            //{
            //    switch (sortOrder)
            //    {
            //        case UsersSortOrder.Id:
            //            allUsers = allUsers.OrderByDescending(x => x.Id).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.FirstName:
            //            allUsers = allUsers.OrderByDescending(x => x.UserName);
            //            break;
            //        case UsersSortOrder.LastName:
            //            allUsers = allUsers.OrderByDescending(x => x.UserNameLast).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.EmailAddress:
            //            allUsers = allUsers.OrderByDescending(x => x.EmailAddress).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.Role:
            //            allUsers = allUsers.OrderByDescending(x => x.Role?.RoleName).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.IsEnabled:
            //            allUsers = allUsers.OrderByDescending(x => x.IsEnabled).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.LastLogin:
            //            allUsers = allUsers.OrderByDescending(x => x.LastLogin).ThenBy(x => x.UserName);
            //            break;
            //        case UsersSortOrder.MemberNumber:
            //            allUsers = allUsers.OrderByDescending(x => x.MemberNumber).ThenBy(x => x.UserName);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            //    }
            //}

            //switch (usersRecordLimit)
            //{
            //    case UsersRecordLimit.Records_10:
            //        allUsers = allUsers.Take(10).ToList();
            //        break;
            //    case UsersRecordLimit.Records_25:
            //        allUsers = allUsers.Take(25).ToList();
            //        break;
            //    case UsersRecordLimit.All:
            //        allUsers = allUsers.ToList();
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException(nameof(usersRecordLimit), usersRecordLimit, null);
            //}

            //return allUsers;
        }

        public int CreateUser(BookUser model)
        {
            model.CreatedOn = DateTime.Now;
            model.UpdatedOn = DateTime.Now;
            _usersRepository.Insert(model);

            var addedUser = GetUserByEmail(model.EmailAddress);
            if (addedUser == null)
                throw new InvalidOperationException("Unable to find user that was added.");

            addedUser.IdHashCode = addedUser.CalculateIdHashCode();
            _usersRepository.UpdateUser(addedUser, true);

            if (addedUser.Id <= 0)
                throw new InvalidOperationException("Unable to find user that was added.");
            return addedUser.Id;
        }

        public BookUser GetUserById(int id)
        {
            return _usersRepository.GetById(id);
        }

        public BookUser GetByIdHashCode(string name)
        {
            return _usersRepository.GetByIdHashCode(name);
        }

        public void UpdateUser(BookUser model)
        {
            model.UpdatedOn = DateTime.Now;
            _usersRepository.UpdateUser(model);
        }

        public IEnumerable<BookUser> GetUsers(int limit, UsersSortOrder sortField, SortDirection sortDirections)
        {
            return _usersRepository.GetUsers(limit, sortField, sortDirections);
        }

        public IEnumerable<BookUser> GetMembersDirectory()
        {
            return _usersRepository.GetMembersDirectory();
        }

        public IEnumerable<BookUser> GetFoundingMembers()
        {
            return _usersRepository.GetFoundingMembers();
        }

        public void UpdateUserLoginTime(int id)
        {
            BookUser user = _usersRepository.GetById(id);

            user.LastLogin = DateTime.Now.ToLocalTime();

            _usersRepository.UpdateUser(user);
        }

        public BookUser GetUserByEmail(string email)
        {
            var users = _usersRepository.GetByEmail(email).ToList();
            if (users.Count > 1)
                throw new InvalidOperationException($"More than one user has same email '{email}'.");

            return users.FirstOrDefault();
        }

        public void DeleteUser(int id)
        {
            _usersRepository.Delete(id);
        }
    }
}
