using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces.Core;
using Elixir.Models.Enums;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    [RoutePrefix("")]
    public class SitemapController : Controller
    {
        private readonly ISiteMapGenerator _siteMapGenerator;

        public SitemapController(ISiteMapGenerator siteMapGenerator)
        {
            _siteMapGenerator = siteMapGenerator;
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            _siteMapGenerator.ServerRootPath = requestContext.HttpContext.Server.MapPath("~");

            return base.BeginExecute(requestContext, callback, state);
        }

        [AllowAnonymous]
        [Route("sitemap.xml")]
        public ActionResult GetRootSitemap()
        {
            var dynamicSitemap = _siteMapGenerator.GenerateRootSiteMap();
            return Content(dynamicSitemap, "text/xml", Encoding.UTF8);
        }
    }
}