using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models
{
    public class ShopOrderProduct
    {
        public virtual int ShopOrderProductId { get; set; }
        public virtual int ShopOrderId { get; set; }
        public virtual ShopProduct ShopProduct { get; set; }
        public virtual int ShopProductId { get; set; }
        public virtual string DnShopProductName { get; set; }
        public virtual int? ShopProductOptionId { get; set; }
        public virtual string DnShopProductOptionName { get; set; }
        public virtual string SKU { get; set; }
        public virtual decimal? PricePaidPerUnit { get; set; }
        public virtual int? Quantity { get; set; }
    }
}
