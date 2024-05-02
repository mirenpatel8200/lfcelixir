using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elixir.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        public string FailedLoginRateText { set; get; }
        public string FailedLoginRateColor { set; get; }

        public int FailedLogin1Hour { set; get; }
        public int FailedLogin24Hours { set; get; }
        public int FailedLogin28Days { set; get; }

        public List<ShopOrder> ShopOrders { get; set; }
        public List<BookUser> Users { get; set; }
    }
}