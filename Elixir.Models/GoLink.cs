using System;

namespace Elixir.Models
{
    public class GoLink : BaseEntity
    {
        public virtual string GoLinkTitle { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string DestinationUrl { get; set; }
        public virtual bool IsBookLink { get; set; }
        public virtual bool IsAffiliateLink { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual string NotesInternal { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime? Updated { get; set; }
    }
}
