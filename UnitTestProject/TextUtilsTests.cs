using Elixir.BusinessLogic.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class TextUtilsTests
    {
        private void GetAlphanumericValue_StringValid(string input)
        {
            var filtered = input.GetAlphanumericValue();

            Assert.AreEqual(input, filtered);
        }

        [TestMethod]
        public void GetAlphanumericValue_StringValidAndLettersOnly_SameString()
        {
            var input = "bngd";
            GetAlphanumericValue_StringValid(input);
        }

        [TestMethod]
        public void GetAlphanumericValue_StringValidAndNumbersOnly_SameString()
        {
            var input = "23122";
            GetAlphanumericValue_StringValid(input);
        }

        [TestMethod]
        public void GetAlphanumericValue_StringValidAndMixed_SameString()
        {
            var input = "awd2dw42w";
            GetAlphanumericValue_StringValid(input);
        }

        [TestMethod]
        public void GetAlphanumericValue_StringInvalidMixedPlus_TrimmedString()
        {
            var input = "bng+awd2";
            var filtered = input.GetAlphanumericValue();

            Assert.AreEqual("bng", filtered);
        }

        [TestMethod]
        public void GetAlphanumericValue_StringInvalidMixedAmpersand_TrimmedString()
        {
            var input = "bng&awd2";
            var filtered = input.GetAlphanumericValue();

            Assert.AreEqual("bng", filtered);
        }

        [TestMethod]
        public void GetAlphanumericValue_StringInvalidFromBeginning_EmptyString()
        {
            var input = "&awd2";
            var filtered = input.GetAlphanumericValue();

            Assert.AreEqual("", filtered);
        }

        [TestMethod]
        public void GetAlphanumericValue_StringHasDashesAndUnderscore_TrimmedString()
        {
            var input = "hey_this-is_test&awd2";
            var filtered = input.GetAlphanumericValue();

            Assert.AreEqual("hey_this-is_test", filtered);
        }

        [TestMethod]
        public void GetAlphanumericValue_StringIsNull_Null()
        {
            string input = null;
            var filtered = input.GetAlphanumericValue();

            Assert.IsNull(filtered);
        }
    }
}
