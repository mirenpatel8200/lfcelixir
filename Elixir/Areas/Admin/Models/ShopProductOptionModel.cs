using Elixir.Models;
using Elixir.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elixir.Areas.Admin.Models
{
    public class ShopProductOptionModel : ShopProductOption
    {
        public ShopProductOptionModel()
        {
            IsEnabled = true;
        }

        public ShopProductOptionModel(ShopProductOption shopProductOption)
        {
            IsEnabled = true;
            ReflectionUtils.ClonePublicProperties(shopProductOption, this);
        }

        [Required(ErrorMessage = "Option Name is required.")]
        [MaxLength(20, ErrorMessage = "Option Name: Length should be less than 20.")]
        public override string OptionName { get; set; }

        [Required(ErrorMessage = "Suffix is required.")]
        [MaxLength(5, ErrorMessage = "Suffix: Length should be less than 5.")]
        public override string SkuSuffix { get; set; }

        [Required(ErrorMessage = "Stock is required.")]
        public override int? StockLevel { get; set; }
    }
}