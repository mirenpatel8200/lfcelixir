using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models.Enums
{
    public enum OrderStatus
    {
        ShoppingCart = 10,
        OrderBeingProcessed = 110,
        PaymentDeclined = 120,
        PartiallyShipped = 150,
        Shipped = 160,
        Delivered = 170,
        Complete = 180,
        Cancelled = 190,
        Other = 199
    }
}
