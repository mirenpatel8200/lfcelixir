using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class UsersRepository : AbstractRepository<BookUser>, IUsersRepository
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ICountryRepository _countryRepository;
        public UsersRepository(IUserRoleRepository userRoleRepository, ICountryRepository countryRepository)
        {
            _userRoleRepository = userRoleRepository;
            _countryRepository = countryRepository;

        }

        public override void Insert(BookUser entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameUsers} ([IsDeleted], [IsEnabled], [IDHashCode], [MemberNumber], [UserNameFirst], [UserNameLast], [UserNameDisplay], [EmailAddress]," +
                    $" [LastLoginDT], [ExpiryDate], [CountryID], [PasswordHash], [PasswordSalt], [PasswordUpdatedDT], [SecurityCode], [SecurityCodeExpiryDT]," +
                    $" [ProfileIsPublic], [ProfileIsMembersOnly], [DescriptionPublic], [Biography], [IsFoundingMember], [WebsiteUrl]," +
                    $" [TwitterUrl], [FacebookUrl], [InstagramUrl], [OtherUrl], [RegistrationMemberType], [NotesAdmin], [UpdatedDT], [UpdatedBy], [CreatedDT], [CreatedBy], [LinkedInUrl], [NewsletterSubscriber])" +
                    $" VALUES (@IsDeleted, @IsEnabled, @IDHashCode, @MemberNumber, @UserNameFirst, @UserNameLast, @UserNameDisplay, @EmailAddress," +
                    $" @LastLoginDT, @ExpiryDate, @CountryID, @PasswordHash, @PasswordSalt, @PasswordUpdatedDT," +
                    $" @SecurityCode, @SecurityCodeExpiryDT, @ProfileIsPublic, @ProfileIsMembersOnly, @DescriptionPublic, @Biography," +
                    $" @IsFoundingMember, @WebsiteUrl, @TwitterUrl, @FacebookUrl, @InstagramUrl, @OtherUrl, @RegistrationMemberType, @NotesAdmin, @UpdatedDT, @UpdatedBy, @CreatedDT, @CreatedBy, @LinkedInUrl, @NewsletterSubscriber)");

                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@IDHashCode", SafeGetStringValue(entity.IdHashCode));
                command.Parameters.AddWithValue("@MemberNumber", entity.MemberNumber.HasValue ? entity.MemberNumber : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserNameFirst", SafeGetStringValue(entity.UserName));
                command.Parameters.AddWithValue("@UserNameLast", SafeGetStringValue(entity.UserNameLast));
                command.Parameters.AddWithValue("@UserNameDisplay", SafeGetStringValue(entity.UserNameDisplay));
                command.Parameters.AddWithValue("@EmailAddress", SafeGetStringValue(entity.EmailAddress));
                command.Parameters.AddWithValue("@LastLoginDT", entity.LastLogin.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.LastLogin) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ExpiryDate", entity.ExpiryDate.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.ExpiryDate) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CountryID", entity.CountryId.HasValue ? entity.CountryId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PasswordHash", SafeGetStringValue(entity.PasswordHash));
                command.Parameters.AddWithValue("@PasswordSalt", SafeGetStringValue(entity.PasswordSalt));
                command.Parameters.AddWithValue("@PasswordUpdatedDT", entity.PasswordUpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.PasswordUpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@SecurityCode", SafeGetStringValue(entity.SecurityCode));
                command.Parameters.AddWithValue("@SecurityCodeExpiryDT", entity.SecurityCodeExpiry.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.SecurityCodeExpiry) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProfileIsPublic", entity.ProfileIsPublic);
                command.Parameters.AddWithValue("@ProfileIsMembersOnly", entity.ProfileIsMembersOnly);
                command.Parameters.AddWithValue("@DescriptionPublic", SafeGetStringValue(entity.DescriptionPublic));
                command.Parameters.AddWithValue("@Biography", SafeGetStringValue(entity.Biography));
                command.Parameters.AddWithValue("@IsFoundingMember", entity.IsFoundingMember);
                command.Parameters.AddWithValue("@WebsiteUrl", SafeGetStringValue(entity.WebsiteUrl));
                command.Parameters.AddWithValue("@TwitterUrl", SafeGetStringValue(entity.TwitterUrl));
                command.Parameters.AddWithValue("@FacebookUrl", SafeGetStringValue(entity.FacebookUrl));
                command.Parameters.AddWithValue("@InstagramUrl", SafeGetStringValue(entity.InstagramUrl));
                command.Parameters.AddWithValue("@OtherUrl", SafeGetStringValue(entity.OtherUrl));
                command.Parameters.AddWithValue("@RegistrationMemberType", entity.RegistrationMemberType.HasValue ? entity.RegistrationMemberType : (object)DBNull.Value);
                command.Parameters.AddWithValue("@NotesAdmin", SafeGetStringValue(entity.AdminNotes));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy.HasValue ? entity.UpdatedBy : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDT", entity.CreatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.CreatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy.HasValue ? entity.CreatedBy : (object)DBNull.Value);
                command.Parameters.AddWithValue("@LinkedInUrl", SafeGetStringValue(entity.LinkedInUrl));
                command.Parameters.AddWithValue("@NewsletterSubscriber", entity.NewsletterSubscriber);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving User to the database");
            }
        }

        public override void Update(BookUser entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(BookUser entity, bool isNewUser = false)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = null;
                if (isNewUser)
                {
                    command = MsAccessDbManager.CreateCommand(
                       $"UPDATE {TableNameUsers} " +
                       $"SET [IsDeleted] = @IsDeleted, [IsEnabled] = @IsEnabled, [IDHashCode] = @IDHashCode, [MemberNumber] = @MemberNumber, [UserNameFirst] = @UserNameFirst, [UserNameLast] = @UserNameLast," +
                       $" [UserNameDisplay] = @UserNameDisplay, [EmailAddress] = @EmailAddress, [LastLoginDT] = @LastLoginDT, [ExpiryDate] = @ExpiryDate, [CountryID] = @CountryID," +
                       $" [PasswordHash] = @PasswordHash, [PasswordSalt] = @PasswordSalt, [PasswordUpdatedDT] = @PasswordUpdatedDT," +
                       $" [SecurityCode] = @SecurityCode, [SecurityCodeExpiryDT] = @SecurityCodeExpiryDT, [ProfileIsPublic] = @ProfileIsPublic, [ProfileIsMembersOnly] = @ProfileIsMembersOnly, [DescriptionPublic] = @DescriptionPublic, [Biography] = @Biography, [IsFoundingMember] = @IsFoundingMember, [WebsiteUrl] = @WebsiteUrl," +
                       $" [TwitterUrl] = @TwitterUrl, [FacebookUrl] = @FacebookUrl, [InstagramUrl] = @InstagramUrl, [OtherUrl] = @OtherUrl, [LinkedInUrl] = @LinkedInUrl, [RegistrationMemberType] = @RegistrationMemberType, [NotesAdmin] = @NotesAdmin, [UpdatedDT] = @UpdatedDT, [UpdatedBy] = @UpdatedBy, [NewsletterSubscriber] = @NewsletterSubscriber" +
                       $" WHERE [UserID] = {entity.Id}");
                }
                else
                {
                    command = MsAccessDbManager.CreateCommand(
                       $"UPDATE {TableNameUsers} " +
                       $"SET [IsDeleted] = @IsDeleted, [IsEnabled] = @IsEnabled, [MemberNumber] = @MemberNumber, [UserNameFirst] = @UserNameFirst, [UserNameLast] = @UserNameLast," +
                       $" [UserNameDisplay] = @UserNameDisplay, [EmailAddress] = @EmailAddress, [LastLoginDT] = @LastLoginDT, [ExpiryDate] = @ExpiryDate, [CountryID] = @CountryID," +
                       $" [PasswordHash] = @PasswordHash, [PasswordSalt] = @PasswordSalt, [PasswordUpdatedDT] = @PasswordUpdatedDT," +
                       $" [SecurityCode] = @SecurityCode, [SecurityCodeExpiryDT] = @SecurityCodeExpiryDT, [ProfileIsPublic] = @ProfileIsPublic, [ProfileIsMembersOnly] = @ProfileIsMembersOnly, [DescriptionPublic] = @DescriptionPublic, [Biography] = @Biography, [IsFoundingMember] = @IsFoundingMember, [WebsiteUrl] = @WebsiteUrl," +
                       $" [TwitterUrl] = @TwitterUrl, [FacebookUrl] = @FacebookUrl, [InstagramUrl] = @InstagramUrl, [OtherUrl] = @OtherUrl, [LinkedInUrl] = @LinkedInUrl , [RegistrationMemberType] = @RegistrationMemberType, [NotesAdmin] = @NotesAdmin, [UpdatedDT] = @UpdatedDT, [UpdatedBy] = @UpdatedBy, [NewsletterSubscriber] = @NewsletterSubscriber" +
                       $" WHERE [UserID] = {entity.Id}");
                }

                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                if (isNewUser)
                    command.Parameters.AddWithValue("@IDHashCode", SafeGetStringValue(entity.IdHashCode));
                command.Parameters.AddWithValue("@MemberNumber", entity.MemberNumber.HasValue ? entity.MemberNumber : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserNameFirst", SafeGetStringValue(entity.UserName));
                command.Parameters.AddWithValue("@UserNameLast", SafeGetStringValue(entity.UserNameLast));
                command.Parameters.AddWithValue("@UserNameDisplay", SafeGetStringValue(entity.UserNameDisplay));
                command.Parameters.AddWithValue("@EmailAddress", SafeGetStringValue(entity.EmailAddress));
                command.Parameters.AddWithValue("@LastLoginDT", entity.LastLogin.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.LastLogin) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ExpiryDate", entity.ExpiryDate.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.ExpiryDate) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CountryID", entity.CountryId.HasValue ? entity.CountryId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PasswordHash", SafeGetStringValue(entity.PasswordHash));
                command.Parameters.AddWithValue("@PasswordSalt", SafeGetStringValue(entity.PasswordSalt));
                command.Parameters.AddWithValue("@PasswordUpdatedDT", entity.PasswordUpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.PasswordUpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@SecurityCode", SafeGetStringValue(entity.SecurityCode));
                command.Parameters.AddWithValue("@SecurityCodeExpiryDT", entity.SecurityCodeExpiry.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.SecurityCodeExpiry) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProfileIsPublic", entity.ProfileIsPublic);
                command.Parameters.AddWithValue("@ProfileIsMembersOnly", entity.ProfileIsMembersOnly);
                command.Parameters.AddWithValue("@DescriptionPublic", SafeGetStringValue(entity.DescriptionPublic));
                command.Parameters.AddWithValue("@Biography", SafeGetStringValue(entity.Biography));
                command.Parameters.AddWithValue("@IsFoundingMember", entity.IsFoundingMember);
                command.Parameters.AddWithValue("@WebsiteUrl", SafeGetStringValue(entity.WebsiteUrl));
                command.Parameters.AddWithValue("@TwitterUrl", SafeGetStringValue(entity.TwitterUrl));
                command.Parameters.AddWithValue("@FacebookUrl", SafeGetStringValue(entity.FacebookUrl));
                command.Parameters.AddWithValue("@InstagramUrl", SafeGetStringValue(entity.InstagramUrl));
                command.Parameters.AddWithValue("@OtherUrl", SafeGetStringValue(entity.OtherUrl));
                command.Parameters.AddWithValue("@LinkedInUrl", SafeGetStringValue(entity.LinkedInUrl));
                command.Parameters.AddWithValue("@RegistrationMemberType", entity.RegistrationMemberType.HasValue ? entity.RegistrationMemberType : (object)DBNull.Value);
                command.Parameters.AddWithValue("@NotesAdmin", SafeGetStringValue(entity.AdminNotes));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy.HasValue ? entity.UpdatedBy : (object)DBNull.Value);
                command.Parameters.AddWithValue("@NewsletterSubscriber", entity.NewsletterSubscriber);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during updating User in the database");
            }
        }

        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameUsers} SET IsDeleted = 1 WHERE UserID = @UserID");

                command.Parameters.AddWithValue("@UserID", id);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during User deleting from the database");
            }
        }

        public override IEnumerable<BookUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BookUser> GetAllUsers(int? limit, UsersSortOrder sortField, SortDirection sortDirections)
        {
            var records = limit.HasValue ? $"TOP {limit.Value}" : "";
            var sql = $"SELECT {records} u.UserID as 0, u.IsDeleted as 1, u.IsEnabled as 2, u.IDHashCode as 3, u.MemberNumber as 4, u.UserNameFirst as 5, u.UserNameLast as 6, u.UserNameDisplay as 7," +
               $" u.EmailAddress as 8, u.LastLoginDT as 9, u.ExpiryDate as 10, u.CountryID as 11, u.PasswordHash as 12, u.PasswordSalt as 13, u.PasswordUpdatedDT as 14," +
               $" u.SecurityCode as 15, u.SecurityCodeExpiryDT as 16, u.ProfileIsPublic as 17, u.ProfileIsMembersOnly as 18, u.DescriptionPublic as 19, u.Biography as 20," +
               $" u.IsFoundingMember as 21, u.WebsiteUrl as 22, u.TwitterUrl as 23, u.FacebookUrl as 24, u.InstagramUrl as 25, u.OtherUrl as 26, u.RegistrationMemberType as 27, u.NotesAdmin as 28," +
               $" u.UpdatedDT as 29, u.UpdatedBy as 30, u.CreatedDT as 31, u.CreatedBy as 32, u.LinkedInUrl as 33, u.NewsletterSubscriber as 34, c.CountryAutoID as 35, c.CountryName as 36," +
               $" c.ContinentCode as 37, c.CountryID as 38" +
               $" FROM (({TableNameUsers} u)" +
               $" LEFT JOIN {TableNameCountry} c ON u.CountryID = c.CountryAutoID)" +
               $" WHERE u.IsDeleted = FALSE" +
               $" ORDER BY u.{sortField.ToString()} {(sortDirections == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var bookUser = MapBookUser(0);
                if (GetTableValue<int?>(11).HasValue)
                {
                    bookUser.Country = MapCountry(35);
                    bookUser.DnCountryName = bookUser.Country.CountryName;
                }
                var userRoles = _userRoleRepository.GetUserRoles(bookUser.Id).ToList();
                //var country = _countryRepository.GetCountryById(bookUser.CountryId.HasValue ? Convert.ToInt32(bookUser.CountryId) : 0);
                //if (country != null)
                //{
                //    bookUser.DnCountryName = country.CountryName;
                //}
                if (userRoles.Count > 0)
                {
                    bookUser.AllRole = userRoles.Select(x => x.Role).ToList();
                    if (userRoles.Count > 1)
                    {
                        var lowestDisplayOrder = userRoles.Min(x => x.Role.DisplayOrder);
                        bookUser.Role = userRoles.Where(x => x.Role.DisplayOrder == lowestDisplayOrder).Select(x => x.Role).FirstOrDefault();
                    }
                    else
                        bookUser.Role = userRoles[0].Role;
                }
                list.Add(bookUser);
            });
        }

        public IEnumerable<BookUser> GetUsers(int limit, UsersSortOrder sortField, SortDirection sortDirections)
        {
            var sql = $"SELECT TOP {limit} u.UserID as 0, u.IsDeleted as 1, u.IsEnabled as 2, u.IDHashCode as 3, u.MemberNumber as 4, u.UserNameFirst as 5, u.UserNameLast as 6, u.UserNameDisplay as 7," +
               $" u.EmailAddress as 8, u.LastLoginDT as 9, u.ExpiryDate as 10, u.CountryID as 11, u.PasswordHash as 12, u.PasswordSalt as 13, u.PasswordUpdatedDT as 14," +
               $" u.SecurityCode as 15, u.SecurityCodeExpiryDT as 16, u.ProfileIsPublic as 17, u.ProfileIsMembersOnly as 18, u.DescriptionPublic as 19, u.Biography as 20," +
               $" u.IsFoundingMember as 21, u.WebsiteUrl as 22, u.TwitterUrl as 23, u.FacebookUrl as 24, u.InstagramUrl as 25, u.OtherUrl as 26, u.RegistrationMemberType as 27, u.NotesAdmin as 28," +
               $" u.UpdatedDT as 29, u.UpdatedBy as 30, u.CreatedDT as 31, u.CreatedBy as 32, u.LinkedInUrl as 33, u.NewsletterSubscriber as 34" +
               $" FROM {TableNameUsers} u" +
               $" WHERE u.IsDeleted = FALSE" +
               $" ORDER BY u.{sortField.ToString()} {(sortDirections == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var bookUser = MapBookUser(0);
                var userRoles = _userRoleRepository.GetUserRoles(bookUser.Id).ToList();
                if (userRoles.Count > 0)
                {
                    bookUser.AllRole = userRoles.Select(x => x.Role).ToList();
                    if (userRoles.Count > 1)
                    {
                        var lowestDisplayOrder = userRoles.Min(x => x.Role.DisplayOrder);
                        bookUser.Role = userRoles.Where(x => x.Role.DisplayOrder == lowestDisplayOrder).Select(x => x.Role).FirstOrDefault();
                    }
                    else
                        bookUser.Role = userRoles[0].Role;
                }
                list.Add(bookUser);
            });
        }

        public IEnumerable<BookUser> GetMembersDirectory()
        {
            var sql = $"SELECT u.UserID as 0, u.IsDeleted as 1, u.IsEnabled as 2, u.IDHashCode as 3, u.MemberNumber as 4, u.UserNameFirst as 5, u.UserNameLast as 6, u.UserNameDisplay as 7," +
                $" u.EmailAddress as 8, u.LastLoginDT as 9, u.ExpiryDate as 10, u.CountryID as 11, u.PasswordHash as 12, u.PasswordSalt as 13, u.PasswordUpdatedDT as 14," +
                $" u.SecurityCode as 15, u.SecurityCodeExpiryDT as 16, u.ProfileIsPublic as 17, u.ProfileIsMembersOnly as 18, u.DescriptionPublic as 19, u.Biography as 20," +
                $" u.IsFoundingMember as 21, u.WebsiteUrl as 22, u.TwitterUrl as 23, u.FacebookUrl as 24, u.InstagramUrl as 25, u.OtherUrl as 26, u.RegistrationMemberType as 27, u.NotesAdmin as 28," +
                $" u.UpdatedDT as 29, u.UpdatedBy as 30, u.CreatedDT as 31, u.CreatedBy as 32, u.LinkedInUrl as 33, u.NewsletterSubscriber as 34, IIF(LEN(UserNameDisplay)>0, UserNameDisplay, UserNameFirst) AS MemberName" +
                $" FROM {TableNameUsers} u" +
                $" WHERE u.IsDeleted = FALSE AND u.IsEnabled = TRUE" +
                $" ORDER BY 35 ASC";

            return QueryAllData(sql, list =>
            {
                var bookUser = MapBookUser(0);
                var userRoles = _userRoleRepository.GetUserRoles(bookUser.Id).ToList();
                if (userRoles.Count > 0)
                {
                    bookUser.AllRole = userRoles.Select(x => x.Role).ToList();
                    if (userRoles.Count > 1)
                    {
                        var lowestDisplayOrder = userRoles.Min(x => x.Role.DisplayOrder);
                        bookUser.Role = userRoles.Where(x => x.Role.DisplayOrder == lowestDisplayOrder).Select(x => x.Role).FirstOrDefault();
                    }
                    else
                        bookUser.Role = userRoles[0].Role;
                }
                // Only supporters and members are allowed
                var roles = userRoles.Where(x => x.RoleId == (int)Roles.Supporter || x.RoleId == (int)Roles.Longevist).ToList().Count;
                if (roles > 0)
                    list.Add(bookUser);
            });
        }

        public IEnumerable<BookUser> GetFoundingMembers()
        {
            var sql = $"SELECT u.UserID as 0, u.IsDeleted as 1, u.IsEnabled as 2, u.IDHashCode as 3, u.MemberNumber as 4, u.UserNameFirst as 5, u.UserNameLast as 6, u.UserNameDisplay as 7," +
                $" u.EmailAddress as 8, u.LastLoginDT as 9, u.ExpiryDate as 10, u.CountryID as 11, u.PasswordHash as 12, u.PasswordSalt as 13, u.PasswordUpdatedDT as 14," +
                $" u.SecurityCode as 15, u.SecurityCodeExpiryDT as 16, u.ProfileIsPublic as 17, u.ProfileIsMembersOnly as 18, u.DescriptionPublic as 19, u.Biography as 20," +
                $" u.IsFoundingMember as 21, u.WebsiteUrl as 22, u.TwitterUrl as 23, u.FacebookUrl as 24, u.InstagramUrl as 25, u.OtherUrl as 26, u.RegistrationMemberType as 27, u.NotesAdmin as 28," +
                $" u.UpdatedDT as 29, u.UpdatedBy as 30, u.CreatedDT as 31, u.CreatedBy as 32, u.LinkedInUrl as 33, u.NewsletterSubscriber as 34, IIF(LEN(UserNameDisplay)>0, UserNameDisplay, UserNameFirst) AS MemberName" +
                $" FROM {TableNameUsers} u" +
                $" WHERE u.IsDeleted = FALSE AND u.IsEnabled = TRUE AND u.IsFoundingMember = TRUE" +
                $" ORDER BY 35 ASC";

            return QueryAllData(sql, list =>
            {
                var bookUser = MapBookUser(0);
                var userRoles = _userRoleRepository.GetUserRoles(bookUser.Id).ToList();
                if (userRoles.Count > 0)
                {
                    bookUser.AllRole = userRoles.Select(x => x.Role).ToList();
                    if (userRoles.Count > 1)
                    {
                        var lowestDisplayOrder = userRoles.Min(x => x.Role.DisplayOrder);
                        bookUser.Role = userRoles.Where(x => x.Role.DisplayOrder == lowestDisplayOrder).Select(x => x.Role).FirstOrDefault();
                    }
                    else
                        bookUser.Role = userRoles[0].Role;
                }
                // Only supporters and members are allowed
                var roles = userRoles.Where(x => x.RoleId == (int)Roles.Supporter || x.RoleId == (int)Roles.Longevist).ToList().Count;
                if (roles > 0)
                    list.Add(bookUser);
            });
        }

        //public bool IsUserInRole(BookUser user, string role)
        //{
        //    using (MsAccessDbManager = new MsAccessDbManager())
        //    {
        //        OleDbCommand command = MsAccessDbManager.CreateCommand(
        //            $"SELECT COUNT(*) FROM {TableNameUserRoles} r, {TableNameUsers} u WHERE u.UserRoleID = r.UserRoleID AND u.UserID = @UserID AND r.UserRoleName = @UserRoleName AND u.IsDeleted = 0");

        //        command.Parameters.AddWithValue("@UserID", user.Id);
        //        command.Parameters.AddWithValue("@UserRoleName", role);

        //        DataReader = command.ExecuteReader();

        //        int count = 0;

        //        while (DataReader.Read())
        //        {
        //            count = GetTableValue<int>(0);
        //        }

        //        return count == 1;
        //    }
        //}

        public IEnumerable<BookUser> GetByEmail(string email)
        {
            var sql = $"SELECT u.UserID as 0, u.IsDeleted as 1, u.IsEnabled as 2, u.IDHashCode as 3, u.MemberNumber as 4, u.UserNameFirst as 5, u.UserNameLast as 6, u.UserNameDisplay as 7," +
               $" u.EmailAddress as 8, u.LastLoginDT as 9, u.ExpiryDate as 10, u.CountryID as 11, u.PasswordHash as 12, u.PasswordSalt as 13, u.PasswordUpdatedDT as 14," +
               $" u.SecurityCode as 15, u.SecurityCodeExpiryDT as 16, u.ProfileIsPublic as 17, u.ProfileIsMembersOnly as 18, u.DescriptionPublic as 19, u.Biography as 20," +
               $" u.IsFoundingMember as 21, u.WebsiteUrl as 22, u.TwitterUrl as 23, u.FacebookUrl as 24, u.InstagramUrl as 25, u.OtherUrl as 26, u.RegistrationMemberType as 27, u.NotesAdmin as 28," +
               $" u.UpdatedDT as 29, u.UpdatedBy as 30, u.CreatedDT as 31, u.CreatedBy as 32, u.LinkedInUrl as 33, u.NewsletterSubscriber as 34" +
               $" FROM {TableNameUsers} u" +
               $" WHERE u.IsDeleted = FALSE AND u.EmailAddress = '{email}'";

            return QueryAllData(sql, list =>
            {
                var bookUser = MapBookUser(0);
                var userRoles = _userRoleRepository.GetUserRoles(bookUser.Id).ToList();
                if (userRoles.Count > 0)
                {
                    bookUser.AllRole = userRoles.Select(x => x.Role).ToList();
                    if (userRoles.Count > 1)
                    {
                        var lowestDisplayOrder = userRoles.Min(x => x.Role.DisplayOrder);
                        bookUser.Role = userRoles.Where(x => x.Role.DisplayOrder == lowestDisplayOrder).Select(x => x.Role).FirstOrDefault();
                    }
                    else
                        bookUser.Role = userRoles[0].Role;
                }
                list.Add(bookUser);
            });
        }

        public BookUser GetById(int id)
        {
            var sql = $"SELECT u.UserID as 0, u.IsDeleted as 1, u.IsEnabled as 2, u.IDHashCode as 3, u.MemberNumber as 4, u.UserNameFirst as 5, u.UserNameLast as 6, u.UserNameDisplay as 7," +
              $" u.EmailAddress as 8, u.LastLoginDT as 9, u.ExpiryDate as 10, u.CountryID as 11, u.PasswordHash as 12, u.PasswordSalt as 13, u.PasswordUpdatedDT as 14," +
              $" u.SecurityCode as 15, u.SecurityCodeExpiryDT as 16, u.ProfileIsPublic as 17, u.ProfileIsMembersOnly as 18, u.DescriptionPublic as 19, u.Biography as 20," +
              $" u.IsFoundingMember as 21, u.WebsiteUrl as 22, u.TwitterUrl as 23, u.FacebookUrl as 24, u.InstagramUrl as 25, u.OtherUrl as 26, u.RegistrationMemberType as 27, u.NotesAdmin as 28," +
              $" u.UpdatedDT as 29, u.UpdatedBy as 30, u.CreatedDT as 31, u.CreatedBy as 32, u.LinkedInUrl as 33, u.NewsletterSubscriber as 34, c.CountryAutoID as 35, c.CountryName as 36," +
              $" c.ContinentCode as 37, c.CountryID as 38" +
              $" FROM (({TableNameUsers} u)" +
              $" LEFT JOIN {TableNameCountry} c ON u.CountryID = c.CountryAutoID)" +
              $" WHERE u.IsDeleted = FALSE AND u.UserID = {id}";

            return QueryAllData(sql, list =>
            {
                var bookUser = MapBookUser(0);
                if (GetTableValue<int?>(11).HasValue)
                {
                    bookUser.Country = MapCountry(35);
                    bookUser.DnCountryName = bookUser.Country.CountryName;
                }
                //var country = _countryRepository.GetCountryById(bookUser.CountryId.HasValue ? Convert.ToInt32(bookUser.CountryId) : 0);
                //if (country != null)
                //{
                //    bookUser.DnCountryName = country.CountryName;
                //}
                var userRoles = _userRoleRepository.GetUserRoles(bookUser.Id).ToList();
                if (userRoles.Count > 0)
                {
                    bookUser.AllRole = userRoles.Select(x => x.Role).ToList();
                    if (userRoles.Count > 1)
                    {
                        var lowestDisplayOrder = userRoles.Min(x => x.Role.DisplayOrder);
                        bookUser.Role = userRoles.Where(x => x.Role.DisplayOrder == lowestDisplayOrder).Select(x => x.Role).FirstOrDefault();
                    }
                    else
                        bookUser.Role = userRoles[0].Role;
                }
                list.Add(bookUser);
            }).FirstOrDefault();
        }

        public BookUser GetByIdHashCode(string name)
        {
            var sql = $"SELECT u.UserID as 0, u.IsDeleted as 1, u.IsEnabled as 2, u.IDHashCode as 3, u.MemberNumber as 4, u.UserNameFirst as 5, u.UserNameLast as 6, u.UserNameDisplay as 7," +
              $" u.EmailAddress as 8, u.LastLoginDT as 9, u.ExpiryDate as 10, u.CountryID as 11, u.PasswordHash as 12, u.PasswordSalt as 13, u.PasswordUpdatedDT as 14," +
              $" u.SecurityCode as 15, u.SecurityCodeExpiryDT as 16, u.ProfileIsPublic as 17, u.ProfileIsMembersOnly as 18, u.DescriptionPublic as 19, u.Biography as 20," +
              $" u.IsFoundingMember as 21, u.WebsiteUrl as 22, u.TwitterUrl as 23, u.FacebookUrl as 24, u.InstagramUrl as 25, u.OtherUrl as 26, u.RegistrationMemberType as 27, u.NotesAdmin as 28," +
              $" u.UpdatedDT as 29, u.UpdatedBy as 30, u.CreatedDT as 31, u.CreatedBy as 32, u.LinkedInUrl as 33, u.NewsletterSubscriber as 34" +
              $" FROM {TableNameUsers} u" +
              $" WHERE u.IsDeleted = FALSE AND u.IDHashCode = '{name}'";

            return QueryAllData(sql, list =>
            {
                var bookUser = MapBookUser(0);
                var userRoles = _userRoleRepository.GetUserRoles(bookUser.Id).ToList();
                if (userRoles.Count > 0)
                {
                    bookUser.AllRole = userRoles.Select(x => x.Role).ToList();
                    if (userRoles.Count > 1)
                    {
                        var lowestDisplayOrder = userRoles.Min(x => x.Role.DisplayOrder);
                        bookUser.Role = userRoles.Where(x => x.Role.DisplayOrder == lowestDisplayOrder).Select(x => x.Role).FirstOrDefault();
                    }
                    else
                        bookUser.Role = userRoles[0].Role;
                }
                list.Add(bookUser);
            }).FirstOrDefault();
        }
    }
}
