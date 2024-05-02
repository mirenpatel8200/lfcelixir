using System;
using System.Web.Mvc;
using System.Web.Routing;
using Elixir.Areas.Admin.ViewModels;
using Elixir.Attributes;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.Contracts.Interfaces.Core;
using Elixir.Models.Enums;
using Elixir.Models.Utils;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    public class WebSiteController : Controller
    {
        private readonly ISiteMapGenerator _siteMapGenerator;
        private readonly string _simpleHash = SHA.GenerateSHA256String("helloelixir");

        public WebSiteController(ISiteMapGenerator siteMapGenerator)
        {
            _siteMapGenerator = siteMapGenerator;
        }

        public ActionResult Index()
        {
            var vm = new WebSiteViewModel
            {
                SitemapInfo = _siteMapGenerator.GetSitemapInfo(),
                SiteMapUrl = _siteMapGenerator.GetSiteMapUrl(),
                ValidationHash = _simpleHash
            };

            return View(vm);
        }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            _siteMapGenerator.ServerRootPath = requestContext.HttpContext.Server.MapPath("~");

            return base.BeginExecute(requestContext, callback, state);
        }

        public ActionResult GenerateSiteMap(string hash)
        {
            if (string.IsNullOrWhiteSpace(_simpleHash) || !hash.Equals(_simpleHash))
                throw new InvalidOperationException("Hashes don't match, please generate sitemap.xml from Admin panel.");

            _siteMapGenerator.SaveSiteMaps();

            return Content("Sitemap files were generated.");
        }
    }
}