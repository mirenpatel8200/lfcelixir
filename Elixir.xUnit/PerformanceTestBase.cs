using System;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Elixir.xUnit
{
    public class PerformanceTestBase : UnitTestBase
    {
        protected PerformanceTestBase(ITestOutputHelper output) : base(output)
        {
        }

        protected TimeSpan MeasureTime(Action testAction, int timesToRun = 1)
        {
            if(testAction == null)
                throw new ArgumentNullException(nameof(testAction));

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < timesToRun; i++)
            {
                testAction();
            }

            sw.Stop();

            return sw.Elapsed;
        }
    }
}
