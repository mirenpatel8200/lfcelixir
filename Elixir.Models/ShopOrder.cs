using Elixir.Models.Enums;
using Elixir.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models
{
    public class ShopOrder
    {
        public virtual int ShopOrderId { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual string IDHashCode { get; set; }
        public virtual int? StatusId { get; set; }
        public virtual int? UserId { get; set; }
        public virtual DateTime? OrderPlacedOn { get; set; }
        public virtual string OrderPlacedIPAddressString { get; set; }
        public virtual DateTime? OrderDespatchedOn { get; set; }
        public virtual string NotesPublic { get; set; }
        public virtual string NotesInternal { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual int? CreatedByUserId { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
        public virtual int? UpdatedByUserId { get; set; }
        public virtual int? ItemsTotal { get; set; }
        public virtual decimal? ShippingPricePaid { get; set; }
        public virtual decimal? PricePaidTotal { get; set; }
        public virtual string EmailAddress { get; set; }
        public virtual string AddressNameFirst { get; set; }
        public virtual string AddressNameLast { get; set; }
        public virtual string AddressLine1 { get; set; }
        public virtual string AddressLine2 { get; set; }
        public virtual string AddressLine3 { get; set; }
        public virtual string AddressTown { get; set; }
        public virtual string AddressPostcode { get; set; }
        public virtual int? AddressCountryID { get; set; }
        public virtual string TelephoneNumber { get; set; }

        public string CalculateIdHashCode()
        {
            var date = CreatedOn;
            var yearSub = Convert.ToDateTime(date).Year - 2000;
            var year = yearSub >= 0 ? yearSub : 0;
            var yy = year.ToSimple2CharsBase26();
            var dd = (31 * (Convert.ToDateTime(date).Month - 1) + Convert.ToDateTime(date).Day).ToSimple2CharsBase26();
            var h = Convert.ToDateTime(date).Hour.ToABasedLetter();
            var m = (Convert.ToDateTime(date).Minute / 3).ToABasedLetter();
            var s = (Convert.ToDateTime(date).Second / 3).ToABasedLetter();

            var sId = ShopOrderId.ToString("D3");
            var nnn = sId.Length > 3 ? sId.Substring(sId.Length - 3) : sId;

            var sb = new StringBuilder();
            sb.Append(yy).Append(dd).Append(h).Append(m).Append(s).Append(nnn);

            return sb.ToString();
        }

        public string OrderStatusName
        {
            get
            {
                switch (StatusId)
                {
                    case (int)OrderStatus.ShoppingCart:
                        return "Shopping cart";
                    case (int)OrderStatus.OrderBeingProcessed:
                        return "Order being processed";
                    case (int)OrderStatus.PaymentDeclined:
                        return "Payment declined";
                    case (int)OrderStatus.PartiallyShipped:
                        return "Partially shipped";
                    case (int)OrderStatus.Shipped:
                        return "Shipped";
                    case (int)OrderStatus.Delivered:
                        return "Delivered";
                    case (int)OrderStatus.Complete:
                        return "Complete";
                    case (int)OrderStatus.Cancelled:
                        return "Cancelled";
                    case (int)OrderStatus.Other:
                        return "Other";
                    default:
                        return null;
                }
            }
        }

    }
}
