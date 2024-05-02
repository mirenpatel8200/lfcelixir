using System.Text;
using System.Web.Mvc;
using Elixir.Contracts.Interfaces.Core;

namespace Elixir.Controllers
{
    [RoutePrefix("")]
    public class RobotsController : Controller
    {
        private readonly IRobotsGenerator _robotsGenerator;

        public RobotsController(IRobotsGenerator robotsGenerator)
        {
            _robotsGenerator = robotsGenerator;
        }

        [AllowAnonymous]
        [Route("robots.txt")]
        public ActionResult GetRobots()
        {
            var url = Request.Url?.Host;
            var robotsContent = _robotsGenerator.GenerateRobots(url, s => Server.MapPath(s));

            return Content(robotsContent, "text/plain", Encoding.UTF8);
        }
    }
}