using System;
using Elixir.Models;
using Elixir.Utils;
using Xunit;

namespace Elixir.xUnit.UnitTests.Core
{
    public class ArticlesHashTests
    {
        private Article CreateArticle(int id, string dateTimeString)
        {
            return new Article()
            {
                Id = id,
                ArticleDate = DateTime.Parse(dateTimeString)
            };
        }

        [Fact]
        public void CalculateIdHash_Test()
        {
            RunHashTest(CreateArticle(13000, "12/31/2018 00:12:34"), "asoiael000");
            RunHashTest(CreateArticle(13000, "12/31/2018 12:34:56"), "asoimls000");
            RunHashTest(CreateArticle(13000, "12/31/2018 23:45:00"), "asoixpa000");
            RunHashTest(CreateArticle(12345, "02/28/2018 10:10:10"), "aschkdd345");
            RunHashTest(CreateArticle(12, "02/28/2018 01:01:01"), "aschbaa012");
            RunHashTest(CreateArticle(1, "02/28/2018 01:01:01"), "aschbaa001");
            RunHashTest(CreateArticle(0, "02/28/2018 01:01:01"), "aschbaa000");

            // Auto-generated tests from Adrian's excel file.
            // =CONCATENATE("RunHashTest(CreateArticle(",B2,", """, TEXT(A2,"mm/dd/yyyy hh:mm:ss"), """), ", """", Y2, """", ");")
            RunHashTest(CreateArticle(12345, "01/01/2000 00:00:00"), "aaabaaa345");
            RunHashTest(CreateArticle(12345, "01/01/1999 00:00:00"), "aaabaaa345");
            RunHashTest(CreateArticle(12345, "12/21/2000 00:00:00"), "aanyaaa345");
            RunHashTest(CreateArticle(12345, "12/21/2001 00:00:00"), "abnyaaa345");
            RunHashTest(CreateArticle(12345, "12/21/2018 00:00:00"), "asnyaaa345");
            RunHashTest(CreateArticle(12345, "12/21/2019 00:00:00"), "atnyaaa345");
            RunHashTest(CreateArticle(12345, "12/21/2025 00:00:00"), "aznyaaa345");
            RunHashTest(CreateArticle(12345, "12/21/2026 00:00:00"), "banyaaa345");
            RunHashTest(CreateArticle(12345, "12/21/2027 00:00:00"), "bbnyaaa345");
            RunHashTest(CreateArticle(12345, "12/21/2028 00:00:00"), "bcnyaaa345");
            RunHashTest(CreateArticle(12345, "01/01/2018 00:00:00"), "asabaaa345");
            RunHashTest(CreateArticle(12345, "01/02/2018 00:00:00"), "asacaaa345");
            RunHashTest(CreateArticle(12345, "01/31/2018 00:00:00"), "asbfaaa345");
            RunHashTest(CreateArticle(12345, "02/01/2018 00:00:00"), "asbgaaa345");
            RunHashTest(CreateArticle(12345, "02/28/2018 00:00:00"), "aschaaa345");
            RunHashTest(CreateArticle(12345, "03/01/2018 00:00:00"), "asclaaa345");
            RunHashTest(CreateArticle(12345, "12/31/2018 00:00:00"), "asoiaaa345");
            RunHashTest(CreateArticle(12345, "12/31/2018 01:00:00"), "asoibaa345");
            RunHashTest(CreateArticle(12345, "12/31/2018 10:00:00"), "asoikaa345");
            RunHashTest(CreateArticle(12345, "12/31/2018 12:00:00"), "asoimaa345");
            RunHashTest(CreateArticle(12345, "12/31/2018 13:00:00"), "asoinaa345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:00:00"), "asoixaa345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:01:00"), "asoixaa345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:02:00"), "asoixaa345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:03:00"), "asoixba345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:58:00"), "asoixta345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:59:00"), "asoixta345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:59:01"), "asoixta345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:59:02"), "asoixta345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:59:03"), "asoixtb345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:59:59"), "asoixtt345");
            RunHashTest(CreateArticle(12345, "12/31/2018 23:59:59"), "asoixtt345");
            RunHashTest(CreateArticle(1, "12/31/2018 23:59:59"), "asoixtt001");
            RunHashTest(CreateArticle(2, "12/31/2018 23:59:59"), "asoixtt002");
            RunHashTest(CreateArticle(12000, "12/31/2018 23:59:59"), "asoixtt000");
            RunHashTest(CreateArticle(12010, "12/31/2018 23:59:59"), "asoixtt010");
            RunHashTest(CreateArticle(12999, "12/31/2018 23:59:59"), "asoixtt999");
            RunHashTest(CreateArticle(13000, "12/31/2018 00:12:34"), "asoiael000");
            RunHashTest(CreateArticle(13000, "12/31/2018 12:34:56"), "asoimls000");
            RunHashTest(CreateArticle(13000, "12/31/2018 23:45:00"), "asoixpa000");
            RunHashTest(CreateArticle(12345, "02/28/2018 10:10:10"), "aschkdd345");
        }

        private void RunHashTest(Article a, string expectedHash)
        {
            var hash = a.CalculateIdHashCode();
            Assert.Equal(expectedHash, hash);
        }

        [Theory]
        [InlineData(1, "ab")]
        [InlineData(26, "ba")]
        [InlineData(27, "bb")]
        [InlineData(18, "as")]
        [InlineData(372, "oi")]
        [InlineData(675, "zz")]
        public void Base26Tests(int i, string s)
        {
            Assert.Equal(s, i.ToSimple2CharsBase26());
        }

        [Theory]
        [InlineData(0, 'a')]
        [InlineData(1, 'b')]
        [InlineData(23, 'x')]
        public void Base26SingleDigitTest(int i, char s)
        {
            Assert.Equal(s, i.ToABasedLetter());
        }
    }
}
