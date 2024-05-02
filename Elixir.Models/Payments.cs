using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models
{
    public class Payments
    {
        public virtual int PaymentId { get; set; }
        public virtual string PaymentReference { get; set; }
        public virtual int? PaymentStatusId { get; set; }
        public virtual decimal? Amount { get; set; }
        public virtual string ProcessorName { get; set; }
        public virtual int? ShopOrderId { get; set; }
        public virtual string NotesUser { get; set; }
        public virtual string NotesAdmin { get; set; }
        public virtual DateTime? PaymentDate { get; set; }
        public virtual string PaymentResponse { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
        public virtual int? CreatedBy { get; set; }
        public virtual int? UpdatedBy { get; set; }
    }
}
