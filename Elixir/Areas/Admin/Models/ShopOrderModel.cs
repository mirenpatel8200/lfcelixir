using Elixir.Models;
using Elixir.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elixir.Areas.Admin.Models
{
    public class ShopOrderModel : ShopOrder
    {
        public ShopOrderModel(ShopOrder shopOrder)
        {
            ReflectionUtils.ClonePublicProperties(shopOrder, this);
        }
    }
}