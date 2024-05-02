using Elixir.Areas.Admin.Models;
using Elixir.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elixir.Areas.Admin.ViewModels
{
    public class CreateEditShopProductOptionViewModel
    {
        public List<ShopProductOptionModel> ShopProductOptionModels { get; set; }
        public int ShopProductId { get; set; }
        public string ShopProductName { get; set; }
        public string OptionsUnit { get; set; }
    }
}