using Elixir.App_Start;
using Elixir.Contracts.Interfaces.Database;
using Microsoft.Practices.Unity;
using Xunit.Abstractions;

namespace Elixir.xUnit
{
    public abstract class UnitTestBase
    {
        protected ITestOutputHelper Output;
        protected UnitTestBase(ITestOutputHelper output)
        {
            Output = output;
        }

        protected IDbManager UnitTestDbManager;

        protected IUnityContainer SetupUnityContainer()
        {
            UnityWebActivator.Start();
            var container = UnityConfig.GetConfiguredContainer();
            container.RegisterType<IDbManager, UnitTestDbManager>();

            return container;
        }
    }
}
