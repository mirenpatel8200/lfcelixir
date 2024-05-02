using System.Web;
using System.Web.Mvc;
using Elixir.ViewModels;

namespace Elixir.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Unauthorized()
        {
            ErrorViewModel errorModel =
                new ErrorViewModel("Not authorized", "Sorry, you are not authorized for this action");

            return View(errorModel);
        }

#if RELEASE
        public ActionResult CatchAllUrls()
        {
            //throwing an exception here pushes the error through the Application_Error method for centralised handling/logging

            throw new HttpException(404, "Page not found.");
        }
#endif
    }
}