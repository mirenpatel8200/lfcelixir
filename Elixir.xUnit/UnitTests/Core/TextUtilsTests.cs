using System;
using Elixir.BusinessLogic.Core.Utils;
using Xunit;

namespace Elixir.xUnit.UnitTests.Core
{
    public class TextUtilsTests
    {
        [Theory]
        [InlineData("Ray Kurzweil", "ray-kurzweil")]
        [InlineData("3D-printed pill approved by FDA", "3d-printed-pill-approved-by-fda")]
        [InlineData("$160 million to develop artificial organs", "160-million-to-develop-artificial-organs")]
        [InlineData("Drug \"melts away\" fat inside arteries", "drug-melts-away-fat-inside-arteries")]
        [InlineData("Drug “melts away” fat inside arteries", "drug-melts-away-fat-inside-arteries")]
        [InlineData("What does heart's ticking mean ?", "what-does-hearts-ticking-mean")]
        [InlineData("No alcohol safe - result from global study", "no-alcohol-safe-result-from-global-study")]
        [InlineData("This#!is---a&test--", "this-is-a-test")]
        [InlineData("a", "a")]
        [InlineData("", "")]
        public void ConvertToUrlName_AllTests(string text, string expected)
        {
            var actual = TextUtils.ConvertToUrlName(text);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertToUrlName_ArgumentIsNull_ExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() => { TextUtils.ConvertToUrlName(null); });
            
        }

        [Theory]
        [InlineData("hello----this--is-text---", "hello-this-is-text-")]
        [InlineData("---hello----this--is-text---", "-hello-this-is-text-")]
        [InlineData("-------this--is-text---", "-this-is-text-")]
        [InlineData("-------t---", "-t-")]
        [InlineData("--t", "-t")]
        [InlineData("-t", "-t")]
        [InlineData("t", "t")]
        [InlineData("-----", "-")]
        [InlineData("-", "-")]
        [InlineData("", "")]
        public void CollapseRepeated_DashTests(string text, string expected)
        {
            var actual = text.CollapseRepeated('-');

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CollapseRepeated_ArgumentIsNull_ThrowsNullRefException()
        {
            string text = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                text.CollapseRepeated('-');
            });
        }
    }
}
