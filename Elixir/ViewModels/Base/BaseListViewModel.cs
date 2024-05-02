using System.Collections.Generic;

namespace Elixir.ViewModels.Base
{
    public class BaseListViewModel<TModel>
    {
        public IEnumerable<TModel> Models { get; set; }
    }
}