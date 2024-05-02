using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models.Identity;

namespace Elixir
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Bootstrapper.Initialise();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // https://stackoverflow.com/a/25844913/10027214
            // Error logging omitted.

            //throw new Exception("TESTING");

#if !DEBUG
            HandleLastException();
#endif
        }

        private void HandleLastException()
        {
            var exception = Server.GetLastError();

            var httpException = exception as HttpException ?? new HttpException(500, "");

            var routeData = new RouteData();
            routeData.Values.Add("ex", exception);

            var webPageProcessor = DependencyResolver.Current.GetService<IWebPagesProcessor>();
            var articlesProcessor = DependencyResolver.Current.GetService<IArticlesProcessor>();
            var topicsProcessor = DependencyResolver.Current.GetService<ITopicsProcessor>();
            var pageXTopicProcessor = DependencyResolver.Current.GetService<IWebPageXTopicProcessor>();
            var blogPostsProcessor = DependencyResolver.Current.GetService<IBlogPostsProcessor>();
            var resourcesProcessor = DependencyResolver.Current.GetService<IResourcesProcessor>();
            var shopProductProcessor = DependencyResolver.Current.GetService<IShopProductProcessor>();
            var shopOrderProcessor = DependencyResolver.Current.GetService<IShopOrderProcessor>();
            var shopOrderProductProcessor = DependencyResolver.Current.GetService<IShopOrderProductProcessor>();
            var usersProcessor = DependencyResolver.Current.GetService<IUsersProcessor>();
            var countryProcessor = DependencyResolver.Current.GetService<ICountryProcessor>();
            var auditLogsProcessor = DependencyResolver.Current.GetService<IAuditLogsProcessor>();
            var userRoleProcessor = DependencyResolver.Current.GetService<IUserRoleProcessor>();
            var settingsProcessor = DependencyResolver.Current.GetService<ISettingsProcessor>();
            IController errorController = new WebPageVisualController(webPageProcessor, articlesProcessor,
                topicsProcessor, pageXTopicProcessor, blogPostsProcessor, resourcesProcessor, shopProductProcessor,
                shopOrderProcessor, shopOrderProductProcessor, usersProcessor, countryProcessor, auditLogsProcessor, userRoleProcessor, settingsProcessor);

            routeData.Values.Add("controller", "WebPageVisual");
            routeData.Values.Add("action", "Index");
            routeData.Values.Add("area", "");

            Response.Clear();
            Server.ClearError();

            switch (httpException.GetHttpCode())
            {
                case 404:
                    routeData.Values.Add("name", "404");
                    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                    break;
                default:
                    routeData.Values.Add("name", "500");
                    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                    break;
            }
        }
    }
}
