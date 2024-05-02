using System.Linq;
using Elixir.Models.Enums;
using Elixir.Utils;
using Xunit;

namespace Elixir.xUnit.UnitTests.Utils
{
    public class EnumUtilsTests
    {
        [Fact]
        public void HasMultipleFlags_EnumHas2Flags_MultipleFlagsIsTrue()
        {
            var e = ResourceTypes.Creation | ResourceTypes.Person;
            Assert.True(e.HasMultipleFlags());
        }

        [Fact]
        public void HasMultipleFlags_EnumHas1Flag_MultipleFlagsIsFalse()
        {
            var e = ResourceTypes.Organisation;
            Assert.False(e.HasMultipleFlags());
        }

        [Fact]
        public void GetFlags_EnumHas2Flags_TheyAreReturned()
        {
            var e = ResourceTypes.Creation | ResourceTypes.Person;
            var flags = e.GetFlags().ToList();

            Assert.True(flags.Contains(ResourceTypes.Creation) && flags.Contains(ResourceTypes.Person));
            Assert.True(flags.Count == 2);
        }
    }
}
