using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class UserRoleRepository : AbstractRepository<UserRole>, IUserRoleRepository
    {
        public override void Insert(UserRole entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameUserRole} (UserID, RoleID)" +
                    $" VALUES (@UserID, @RoleID)");

                command.Parameters.AddWithValue("@UserID", entity.UserId.HasValue ? entity.UserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@RoleID", entity.RoleId.HasValue ? entity.RoleId : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving user role to the database");
            }
        }

        public override void Update(UserRole entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameUserRole} SET" +
                    $" UserID = @UserID, RoleID = @RoleID" +
                    $" WHERE UserRoleID = {entity.UserRoleId}");

                command.Parameters.AddWithValue("@UserID", entity.UserId.HasValue ? entity.UserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@RoleID", entity.RoleId.HasValue ? entity.RoleId : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing user role in the database");
            }
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<UserRole> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserRole> GetUserRoles(int userId)
        {
            var sql = $"SELECT ur.UserRoleID as 0, ur.UserID as 1, ur.RoleID as 2," +
                $" r.RoleID as 3, r.RoleName as 4, r.LoginRedirectUrl as 5, r.DisplayOrder as 6" +
                $" FROM {TableNameUserRole} ur" +
                $" LEFT JOIN {TableNameRole} r ON r.RoleID = ur.RoleID" +
                $" WHERE ur.UserID = {userId}";

            return QueryAllData(sql, list =>
            {
                var userRole = MapUserRole(0);
                if (GetTableValue<int?>(2).HasValue)
                    userRole.Role = MapRole(3);
                list.Add(userRole);
            });
        }

        public UserRole GetUserRole(int userId, int roleId)
        {
            var sql = $"SELECT ur.UserRoleID as 0, ur.UserID as 1, ur.RoleID as 2," +
                $" r.RoleID as 3, r.RoleName as 4, r.LoginRedirectUrl as 5, r.DisplayOrder as 6" +
                $" FROM {TableNameUserRole} ur" +
                $" LEFT JOIN {TableNameRole} r ON r.RoleID = ur.RoleID" +
                $" WHERE ur.UserID = {userId} AND ur.RoleID = {roleId}";

            return QueryAllData(sql, list =>
            {
                var userRole = MapUserRole(0);
                if (GetTableValue<int?>(2).HasValue)
                    userRole.Role = MapRole(3);
                list.Add(userRole);
            }).FirstOrDefault();
        }
    }
}
