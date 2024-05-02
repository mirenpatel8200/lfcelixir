using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models
{
    public class ShopProduct
    {
        public virtual int ShopProductId { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual string ShopProductName { get; set; }
        public virtual string UrlName { get; set; }
        public virtual string SKU { get; set; }
        public virtual string ShopProductDescription { get; set; }
        public virtual string ContentMain { get; set; }
        public virtual ShopCategory ShopCategory { get; set; }
        public virtual int? ShopCategoryId { get; set; }
        public virtual decimal? PriceRRP { get; set; }
        public virtual decimal? PriceLongevist { get; set; }
        public virtual decimal? ShippingPrice { get; set; }
        public virtual bool IsLongevistsOnly { get; set; }
        public virtual int? StockLevel { get; set; }
        public virtual int? DisplayOrder { get; set; }
        public virtual string OptionsUnit { get; set; }
        public virtual string ImageThumb { get; set; }
        public virtual string ImageMain { get; set; }
        public virtual string NotesInternal { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
        public virtual int? CreatedBy { get; set; }
        public virtual int? UpdatedBy { get; set; }
        public virtual string LastUpdatedBy { get; set; }
    }
}
