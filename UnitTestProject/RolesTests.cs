using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.DataAccess.Repositories.MsAccess;
using Elixir.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class RolesTests
    {

        [TestMethod]
        public void GetAllUserRoles()
        {
            IUserRolesRepository rolesRepository = new RolesRepository();

            BookUserRole[] roles = rolesRepository.GetAll().ToArray();

            Assert.IsTrue(true);
        }
    }
}
