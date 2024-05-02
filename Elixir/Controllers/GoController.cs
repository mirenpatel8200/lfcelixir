using System.Web.Mvc;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.Contracts.Interfaces;
using Elixir.Models.Exceptions;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class GoController : Controller
    {
        private readonly IGoLinksProcessor _goLinksProcessor;
        private readonly IGoLinkLogsProcessor _goLinkLogsProcessor;
        private readonly ISettingsProcessor _settingsProcessor;

        public GoController(IGoLinksProcessor goLinksProcessor, IGoLinkLogsProcessor goLinkLogsProcessor, ISettingsProcessor settingsProcessor)
        {
            _goLinksProcessor = goLinksProcessor;
            _goLinkLogsProcessor = goLinkLogsProcessor;
            _settingsProcessor = settingsProcessor;
        }

        public ActionResult Index(string shortCode)
        {
            var validCode = shortCode.GetAlphanumericValue();
            var goLink = _goLinksProcessor.GetByShortUrl(validCode);
            if(goLink == null)
                throw new ContentNotFoundException("Go links is not found.");

            var ipAddress = Request.UserHostAddress;
            _goLinkLogsProcessor.Log(goLink.Id.Value, ipAddress);

            return RedirectPermanent(goLink.DestinationUrl);
        }
    }
}