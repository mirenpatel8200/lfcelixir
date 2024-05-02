using Elixir.Areas.AdminManual.Models;
using Elixir.Models;
using Elixir.Utils.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UtilsTests
    {
        [TestMethod]
        public void TestClonePublicProperties()
        {
            BookPage page = new BookPage();
            page.Id = new int?(1);
            page.BookPageName = "Test";
            page.BookSection = new BookSection("Test Section");
            page.IsIncluded = true;
            page.LifeExtension40 = 40;
            page.Notes = "Notes";
            page.DisplayOrder = 9;
            BookPageModel pageModel = new BookPageModel(); 

            ReflectionUtils.ClonePublicProperties(page, pageModel);
        }
    }
}
