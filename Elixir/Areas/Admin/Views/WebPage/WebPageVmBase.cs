using System.Collections.Generic;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;

namespace Elixir.Areas.Admin.Views.WebPage
{
    public abstract class WebPageVmBase : WebPageModel
    {
        public IEnumerable<KeyValuePair<int, string>> AvailableParentPages { get; set; }

        public IEnumerable<KeyValuePair<int, string>> AvailableWebPageTypes { get; set; }

        public IEnumerable<SelectListItem> PrimaryTopics { get; set; }

        public IEnumerable<SelectListItem> SecondaryTopics { get; set; }

        public bool IsSignificantChange { get; set; }
    }
}