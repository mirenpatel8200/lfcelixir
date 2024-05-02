using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class FileController : Controller
    {
        // GET: File
        [Route("MembersOnly/{name}")]
        public ActionResult MembersOnly(string name="")
        {
            try
            {
                if (User.Identity.IsAuthenticated && (User.IsInRole(Roles.Administrator.ToString()) || User.IsInRole(Roles.Longevist.ToString())))
                {
                    var FolderName = Server.MapPath("/members-only");
                    var FilePath = Path.Combine(FolderName, name + ".jpg");
                    return base.File(FilePath, "image/jpeg");
                }
            }
            catch (Exception ex)
            {
            }
            var _FolderName = Server.MapPath("/members-only");
            var _FilePath = Path.Combine(_FolderName, "404.jpg");
            return base.File(_FilePath, "image/jpeg");
        }
    }
}