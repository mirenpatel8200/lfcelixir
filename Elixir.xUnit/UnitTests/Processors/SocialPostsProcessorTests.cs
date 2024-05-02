using System;
using Elixir.BusinessLogic.Processors.SocialPosts;
using Xunit;

namespace Elixir.xUnit.UnitTests.Processors
{
    public class SocialPostsProcessorTests
    {
        [Theory]
        [InlineData("https://www.linkedin.com/company/frontiers/", "frontiers")]
        [InlineData("https://www.linkedin.com/company/frontiers", "frontiers")]
        [InlineData("https://www.linkedin.com/company/digital-health-intelligence-ltd/", "digital-health-intelligence-ltd")]
        [InlineData("https://www.linkedin.com/company/digital-health-intelligence-ltd", "digital-health-intelligence-ltd")]
        [InlineData("https://www.linkedin.com/groups/1863015/#", null)]
        [InlineData("https://www.linken.com/compafrontiers/", null)]
        [InlineData("http://longevityfacts.com/", null)]
        [InlineData("asd", null)]
        public void ExtractLinkedInHandle_CorrectData(string input, string expected)
        {
            var processor = new SocialPostsProcessor(null);
            var actual = processor.ExtractLinkedInHandle(input);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        public void ExtractLinkedInHandle_NotLinkedInWebsite_ThrowsException(string input)
        {
            var processor = new SocialPostsProcessor(null);

            Assert.Throws<ArgumentException>(() =>
            {
                var actual = processor.ExtractLinkedInHandle(input);
            });
        }
    }
}
