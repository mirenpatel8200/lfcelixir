using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.Views.GoLinkLog;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models.Enums;
using Elixir.Models.Utils;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    [RouteArea("Admin")]
    //[RoutePrefix("golink")]
    //[Route("{action}")]
    public class GoLinkLogController : BaseController
    {
        private readonly IGoLinkLogsProcessor _goLinkLogsProcessor;

        public GoLinkLogController(IGoLinkLogsProcessor goLinkLogsProcessor)
        {
            _goLinkLogsProcessor = goLinkLogsProcessor;
        }

        // /admin/golink/log
        [Route("golink/log")]
        public ActionResult Log()
        {
            var logs = _goLinkLogsProcessor.GetLogs();

            var vm = new GoLinkLogsViewModel()
            {
                Models = logs.Select(x => new GoLinkLogModel(x))
            };

            return View(vm);
        }
    }
}