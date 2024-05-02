using System.Web.Mvc;
using Elixir.Models.Json;

namespace Elixir.Controllers.Base
{
    public class JsonController : Controller
    {
        [NonAction]
        protected JsonResult Json(JsonActionResult result)
        {
            return base.Json(result);
        }

        [NonAction]
        protected JsonResult Json<TData>(JsonActionResult<TData> result)
        {
            return base.Json(result);
        }
    }
}