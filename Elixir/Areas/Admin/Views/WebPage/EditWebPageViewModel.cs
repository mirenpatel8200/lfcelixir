using Elixir.Utils.Reflection;

namespace Elixir.Areas.Admin.Views.WebPage
{
    public class EditWebPageViewModel : WebPageVmBase
    {
        internal void EntityToViewModel(Elixir.Models.WebPage webPage)
        {
            ReflectionUtils.ClonePublicProperties(webPage, this);
        }
    }
}