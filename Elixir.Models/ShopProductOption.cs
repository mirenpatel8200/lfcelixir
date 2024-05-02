using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models
{
    public class ShopProductOption
    {
        public virtual int ShopProductOptionId { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual int ShopProductId { get; set; }
        public virtual string OptionName { get; set; }
        public virtual string SkuSuffix { get; set; }
        public virtual decimal? PriceExtra { get; set; }
        public virtual int? StockLevel { get; set; }
        public virtual int? DisplayOrder { get; set; }
        public virtual bool IsDefaultOption { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
        public virtual int? CreatedByUserId { get; set; }
        public virtual int? UpdatedByUserId { get; set; }
    }
}
