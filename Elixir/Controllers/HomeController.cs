using System.Web;
using System.Web.Mvc;
using Elixir.Contracts.Interfaces;
using Elixir.Models;
using Elixir.Models.Exceptions;
using Elixir.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private ISectionsProcessor _sectionsProcessor;
        private IPagesProcessor _pagesProcessor;

        public HomeController(ISectionsProcessor sectionsProcessor, IPagesProcessor pagesProcessor)
        {
            _sectionsProcessor = sectionsProcessor;
            _pagesProcessor = pagesProcessor;
        }
      
        public ActionResult Index()
        {
            return RedirectToAction("Index", "WebPageVisual", new { name = "home" });
        }
    }
}