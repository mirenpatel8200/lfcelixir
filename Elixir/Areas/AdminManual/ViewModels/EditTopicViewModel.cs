using Elixir.Areas.Admin.Models;
using Elixir.Models;
using Elixir.ViewModels.Base;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Elixir.Areas.AdminManual.ViewModels
{
    public class EditTopicViewModel : BaseCUWithSelectViewModel<TopicModel>
    {
        public void FillAvailableWebPages(IEnumerable<WebPage> webPages, int? selectedItemId)
        {
            var allWebPages = webPages.OrderBy(p => p.WebPageName)
                .Select(wp => new SelectListItem { Text = wp.WebPageName, Value = wp.Id.ToString() })
                .ToList();
            allWebPages.Insert(0, new SelectListItem
            {
                Text = "None",
                Value = "0"
            });
            if (selectedItemId.HasValue && selectedItemId.Value != 0)
            {
                allWebPages.Find(p => p.Value == selectedItemId.ToString()).Selected = true;
            }
            else
            {
                allWebPages.Find(p => p.Value == "0").Selected = true;
            }
            this.SelectItems = allWebPages;
        }
    }
}