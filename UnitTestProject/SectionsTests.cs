using System.Collections.Generic;
using System.Linq;
using Elixir.BusinessLogic.Processors;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.DataAccess.Repositories.MsAccess;
using Elixir.Models;
using Elixir.Models.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class SectionsTests
    {
        private ISectionsProcessor _sectionsProcessor;
        private ISectionsRepository _sectionsRepository;

        [TestInitialize]
        public void Initialize()
        {
            _sectionsRepository = new SectionsRepository();

            _sectionsProcessor = new SectionsProcessor(_sectionsRepository);
        }

        [TestMethod]
        public void GetAllSectionsTest()
        {
            List<BookSection> bookSections = _sectionsProcessor.GetAllSections(BookDataType.All, BookSectionsSortOrder.BookSectionID, SortDirection.Ascending).ToList();

            Assert.IsTrue(bookSections.Count > 0);
        }
    }
}
