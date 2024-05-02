using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elixir.ViewModels
{
    public class ShopViewModel : BaseCUWithMultipleSelectViewModel<ShopProduct>
    {
        public ShopProduct ShopProduct { get; set; }
        public List<ShopProductOption> ShopProductOptions { get; set; }
        public List<ShopOrderProduct> ShopOrderProducts { get; set; }
        public ShopNavigationBar ShopNavigationBar { get; set; }
        public bool IsDisableAddToCart { get; set; }
    }
}