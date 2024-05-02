using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Microsoft.AspNet.Identity;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class RolesRepository : AbstractRepository<BookUserRole>, IUserRolesRepository, IRoleStore<BookUserRole, int>
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public override void Insert(BookUserRole entity)
        {
            throw new NotImplementedException();
        }

        public override void Update(BookUserRole entity)
        {
            throw new NotImplementedException();
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<BookUserRole> GetAll()
        {
            //using (MsAccessDbManager = new MsAccessDbManager())
            //{
            //    DataReader = MsAccessDbManager.CreateCommand(
            //        $"SELECT UserRoleID, UserRoleName FROM {TableNameUserRoles}")
            //        .ExecuteReader();

            //    while (DataReader.Read())
            //    {
            //        BookUserRole bookUserRole = new BookUserRole()
            //        {
            //            Id = GetTableValue<int>(0),
            //            Name = GetTableValue<String>(1)
            //        };

            //        yield return bookUserRole;
            //    }
            //}
            throw new NotImplementedException();
        }

        public Task CreateAsync(BookUserRole role)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(BookUserRole role)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(BookUserRole role)
        {
            throw new NotImplementedException();
        }

        public Task<BookUserRole> FindByIdAsync(int roleId)
        {
            throw new NotImplementedException();
        }

        public Task<BookUserRole> FindByNameAsync(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
