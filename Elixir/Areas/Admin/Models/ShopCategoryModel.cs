using Elixir.Models;
using Elixir.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elixir.Areas.Admin.Models
{
    public class ShopCategoryModel : ShopCategory
    {
        public ShopCategoryModel()
        {
            IsEnabled = true;
        }

        public ShopCategoryModel(ShopCategory shopCategory)
        {
            IsEnabled = true;

            if (shopCategory.PrimaryWebPage?.Id != null)
                PrimaryWebPageId = shopCategory.PrimaryWebPage.Id.Value;

            ReflectionUtils.ClonePublicProperties(shopCategory, this);
        }

        [Required(ErrorMessage = "Category Name is required.")]
        [MaxLength(100, ErrorMessage = "Category Name: Length should be less than 100.")]
        public override string ShopCategoryName { get; set; }

        [Required(ErrorMessage = "Primary Web Page is required.")]
        public override int? PrimaryWebPageId { get; set; }

        [MaxLength(255, ErrorMessage = "Notes (internal): Length should be less than 255.")]
        public override string NotesInternal { get; set; }
    }
}