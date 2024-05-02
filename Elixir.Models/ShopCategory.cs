using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models
{
    public class ShopCategory
    {
        public virtual int ShopCategoryId { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual string ShopCategoryName { get; set; }
        public virtual WebPage PrimaryWebPage { get; set; }
        public virtual int? PrimaryWebPageId { get; set; }
        public virtual string DnPrimaryWebPageName { get; set; }
        public virtual string DnPrimaryWebPageUrlName { get; set; }
        public virtual string ImageThumb { get; set; }
        public virtual string ImageMain { get; set; }
        public virtual string NotesInternal { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual DateTime? UpdatedOn { get; set; }
        public virtual int? CreatedByUserId { get; set; }
        public virtual int? UpdatedByUserId { get; set; }
        public virtual string LastUpdatedBy { get; set; }
    }
}
