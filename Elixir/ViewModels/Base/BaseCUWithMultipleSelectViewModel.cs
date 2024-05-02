using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Elixir.ViewModels.Base
{
    // ReSharper disable once InconsistentNaming
    public class BaseCUWithMultipleSelectViewModel<TModel> : BaseCUViewModel<TModel>
    {
        public Dictionary<String, IEnumerable<SelectListItem>> SelectItemsHolder { get; set; }

        protected BaseCUWithMultipleSelectViewModel()
        {
            SelectItemsHolder = new Dictionary<string, IEnumerable<SelectListItem>>();
        }

        public void AddSelectList(String name, IEnumerable<SelectListItem> listItems)
        {
            if(SelectItemsHolder.ContainsKey(name) == false)
                SelectItemsHolder.Add(name, listItems);
            else
                SelectItemsHolder[name] = listItems;
        }

        public IEnumerable<SelectListItem> GetSelectList(String name)
        {
            return SelectItemsHolder.ContainsKey(name) ? SelectItemsHolder[name] : null;
        }

        public bool IsSelectsListEmpty => SelectItemsHolder.Count == 0;
    }
}