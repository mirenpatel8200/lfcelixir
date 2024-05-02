using System.Web.Mvc;
using Elixir.App_Start;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Mvc;

namespace Elixir
{
    public class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = UnityConfig.GetConfiguredContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }
    }
}