using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.DataAccess.Repositories.MsAccess;
using Elixir.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UsersTests
    {
        private IUsersRepository _usersRepository;
        private ICountryRepository _countryRepository;

        public UsersTests(IUserRoleRepository userRoleRepository, ICountryRepository countryRepository)
        {
            _usersRepository = new UsersRepository(userRoleRepository, countryRepository);
        }

        [TestMethod]
        public void GetAllUsers()
        {
            BookUser[] users = _usersRepository.GetAll().ToArray();

            Assert.IsTrue(true);

        }

        [TestMethod]
        public void InsertUser()
        {
            BookUser user = new BookUser()
            {
                AdminNotes = "Notes",
                UserNameLast = "Yasko 2",
                UserName = "Alexander",
                EmailAddress = "someone@example.com",
                IsEnabled = true,
                //BookUserRole = new BookUserRole()
                //{
                //    Id = 0
                //}
            };

            _usersRepository.Insert(user);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void DeleteUser()
        {
            _usersRepository.Delete(3);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void UpdateUser()
        {
            BookUser user = _usersRepository.GetAll().FirstOrDefault(x => x.UserName == "Alexander");

            user.AdminNotes = "New notes from admin";

            _usersRepository.UpdateUser(user);

            Assert.IsTrue(true);
        }
    }
}
