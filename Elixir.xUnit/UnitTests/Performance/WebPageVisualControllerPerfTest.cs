using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Elixir.App_Start;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Database;
using Elixir.Controllers;
using Elixir.Models.Enums;
using Elixir.Models.Identity;
using Microsoft.Practices.Unity;
using Xunit;
using Xunit.Abstractions;

namespace Elixir.xUnit.UnitTests.Performance
{
    public class WebPageVisualControllerPerfTest : PerformanceTestBase
    {
        public WebPageVisualControllerPerfTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Index_LoadingSpeed()
        {
            var container = SetupUnityContainer();

            var t = MeasureTime(() =>
            {
                var controller = new WebPageVisualController(
                    container.Resolve<IWebPagesProcessor>(),
                    container.Resolve<IArticlesProcessor>(),
                    container.Resolve<ITopicsProcessor>(),
                    container.Resolve<IWebPageXTopicProcessor>(),
                    container.Resolve<IBlogPostsProcessor>(),
                    container.Resolve<IResourcesProcessor>(),
                    container.Resolve<IShopProductProcessor>(),
                    container.Resolve<IShopOrderProcessor>(),
                    container.Resolve<IShopOrderProductProcessor>(),
                    container.Resolve<IUsersProcessor>(),
                    container.Resolve<ICountryProcessor>(),
                    container.Resolve<IAuditLogsProcessor>(),
                    container.Resolve<IUserRoleProcessor>(),
                    container.Resolve<ISettingsProcessor>()
                );

                var r = controller.Index("home");
            });

            Output.WriteLine($"Elapsed: {t}");
        }

        [Fact]
        public void Index_ContentMain()
        {
            var container = SetupUnityContainer();
            var wpProc = DependencyResolver.Current.GetService<IWebPagesProcessor>();

            var t = MeasureTime(() =>
            {
                var webPage = wpProc.GetWebPageByUrlName("home", (int)EnumWebPageType.Page);
            });

            Output.WriteLine($"Elapsed: {t}");
        }

        [Fact]
        public void Index_LatestNewsForHomePage()
        {
            var container = SetupUnityContainer();

            var articlesProcessor = DependencyResolver.Current.GetService<IArticlesProcessor>();

            var t = MeasureTime(() =>
            {
                var la = articlesProcessor.GetLatestArticles();
            });

            Output.WriteLine($"Elapsed: {t}");
        }
    }
}
