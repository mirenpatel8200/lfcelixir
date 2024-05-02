using System.Linq;
using Elixir.BusinessLogic.Processors;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.DataAccess.Repositories.MsAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ArticlesTests
    {
        [TestMethod]
        public void TestGetAll()
        {
            IArticlesRepository repository = new ArticlesRepository();
            IArticlesProcessor processor = new ArticlesProcessor(repository, null);

            var list = processor.Get100Articles().ToArray();
        }
    }
}
