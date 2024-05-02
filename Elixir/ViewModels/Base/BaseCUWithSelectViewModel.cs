using System.Collections.Generic;
using System.Web.Mvc;

namespace Elixir.ViewModels.Base
{
    // ReSharper disable once InconsistentNaming
    public class BaseCUWithSelectViewModel<TModel> : BaseCUViewModel<TModel>
    {
        public IEnumerable<SelectListItem> SelectItems { get; set; }
    }
}