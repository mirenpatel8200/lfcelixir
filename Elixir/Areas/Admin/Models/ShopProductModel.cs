using Elixir.Models;
using Elixir.Models.Validation;
using Elixir.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elixir.Areas.Admin.Models
{
    public class ShopProductModel : ShopProduct
    {
        public ShopProductModel()
        {
            IsEnabled = true;
        }

        public ShopProductModel(ShopProduct shopProduct)
        {
            IsEnabled = true;
            if (shopProduct.ShopCategory != null && shopProduct.ShopCategoryId > 0)
                ShopCategoryId = shopProduct.ShopCategory.ShopCategoryId;
            ReflectionUtils.ClonePublicProperties(shopProduct, this);
        }

        [Required(ErrorMessage = "Product Name is required.")]
        [MaxLength(255, ErrorMessage = "Product Name: Length should be less than 255.")]
        public override string ShopProductName { get; set; }

        [Required(ErrorMessage = "URL Name is required.")]
        [MaxLength(255, ErrorMessage = "URL Name: Length should be less than 255.")]
        [IsUrlName]
        public override string UrlName { get; set; }

        [Required(ErrorMessage = "SKU is required.")]
        [MaxLength(255, ErrorMessage = "SKU: Length should be less than 255.")]
        public override string SKU { get; set; }

        [Required(ErrorMessage = "Shop Category is required.")]
        public override int? ShopCategoryId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(100, ErrorMessage = "Description: Length should be less than 100.")]
        public override string ShopProductDescription { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Content Main is required.")]
        public override string ContentMain { get; set; }

        [Required(ErrorMessage = "RRP is required.")]
        public override decimal? PriceRRP { get; set; }

        [MaxLength(255, ErrorMessage = "Main Image: Length should be less than 255.")]
        public override string ImageMain { get; set; }

        [MaxLength(255, ErrorMessage = "Options Unit: Length should be less than 255.")]
        public override string OptionsUnit { get; set; }

        [MaxLength(255, ErrorMessage = "Thumb Image: Length should be less than 255.")]
        public override string ImageThumb { get; set; }

        [MaxLength(255, ErrorMessage = "Notes (internal): Length should be less than 255.")]
        public override string NotesInternal { get; set; }
    }
}