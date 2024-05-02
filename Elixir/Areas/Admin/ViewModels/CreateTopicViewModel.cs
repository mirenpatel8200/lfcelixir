using Elixir.Areas.Admin.Models;
using Elixir.Models;
using Elixir.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Elixir.Areas.Admin.ViewModels
{
    public class CreateTopicViewModel : BaseCUWithSelectViewModel<TopicModel>
    {
        public void FillAvailableWebPages(IEnumerable<WebPage> webPages)
        {
            var availableWebPages = webPages.OrderBy(p => p.WebPageName)
                .Select(wp => new SelectListItem { Text = wp.WebPageName, Value = wp.Id.ToString() })
                .ToList();

            availableWebPages.Insert(0, new SelectListItem { Text = "None", Value = "0", Selected = true });

            this.SelectItems = availableWebPages;
        }
    }
}