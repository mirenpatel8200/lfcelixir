using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elixir.Models
{
    public class ShopNavigationBar
    {
        public decimal TotalAmount { get; set; }
        public int TotalQuantity { get; set; }
        public bool IsOnlyMembershipProducts { get; set; }
    }
}