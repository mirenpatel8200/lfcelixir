using System.Collections.Generic;
using System.Linq;

namespace Elixir.ViewModels.Navigation
{
    public class NavigationItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public IEnumerable<NavigationItem> ChildItems { get; set; }
        public bool HasChildItems => ChildItems != null && ChildItems.Any();
    }
}