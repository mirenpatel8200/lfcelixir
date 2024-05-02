using Elixir.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elixir.Areas.Admin.ViewModels
{
    public class ShopOrderDetailsViewModel
    {
        public ShopOrder ShopOrder { get; set; }
        public List<ShopOrderProduct> ShopOrderProducts { get; set; }
        public decimal TotalAmount { get; set; }
        public List<Payments> Payments { get; set; }
        [MaxLength(255, ErrorMessage = "Notes Public: Length should be less than 100.")]
        public string NotesPublic { get; set; }
        [MaxLength(255, ErrorMessage = "Notes Internal: Length should be less than 100.")]
        public string NotesInternal { get; set; }
        public string IDHashCode { get; set; }

    }
}