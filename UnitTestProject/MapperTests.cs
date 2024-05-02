using Elixir.DataAccess.Repositories;
using Elixir.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class MapperTests
    {
        [TestMethod]
        public void TestMapper()
        {
            var m = new Mapper<GoLink>(null).SetStartIndex(5).Map(x => x.Id).Create();
        }
    }
}
