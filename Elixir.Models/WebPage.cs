
using System;

namespace Elixir.Models
{
    public class WebPage : BaseEntity
    {
        public virtual string UrlName { get; set; }

        public virtual string WebPageName { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual string WebPageTitle { get; set; }

        public virtual string ContentMain { get; set; }

        // TODO: AY - rename to ParentId. Doesn't match C# naming convention.
        public virtual int? ParentID { get; set; }

        public virtual bool IsSubjectPage { get; set; }
        
        public virtual int DisplayOrder { get; set; }

        public virtual bool IsEnabled { get; set; }

        public virtual string NotesInternal { get; set; }

        public virtual string ParentName { get; set; }

        public virtual DateTime CreatedDateTime { get; set; }
        public virtual DateTime? UpdatedDateTime { get; set; }
        public virtual DateTime PublishedOnDT { get; set; }
        public virtual DateTime? PublishedUpdatedDT { get; set; }

        public virtual string BannerImageFileName { get; set; }
        public virtual string SocialImageFileName { get; set; }
        public virtual string MetaDescription { get; set; }

        public virtual int PrimaryTopicID { get; set; }
        public virtual Topic PrimaryTopic { get; set; }
        public virtual int? SecondaryTopicID { get; set; } 
        public virtual Topic SecondaryTopic { get; set; }

        public virtual int TypeID { get; set; }
        public virtual string TypeName { get; set; }

        //Added By Harmeet
        public virtual int CreatedByUserId { get; set; }
        public virtual int UpdatedByUserId { get; set; }


        public string GetBannerImageName(string defaultName)
        {
            var bannerImageName = defaultName;
            if (!string.IsNullOrWhiteSpace(this.BannerImageFileName))
                bannerImageName = this.BannerImageFileName;
            else if (this.PrimaryTopic != null && !string.IsNullOrWhiteSpace(this.PrimaryTopic.BannerImageFileName))
                bannerImageName = this.PrimaryTopic.BannerImageFileName;

            return bannerImageName;
        }

        public string GetSocialImageName(string defaultName)
        {
            var siName = defaultName;
            if (!string.IsNullOrWhiteSpace(SocialImageFileName))
                siName = SocialImageFileName;
            else if (PrimaryTopic != null && !string.IsNullOrWhiteSpace(PrimaryTopic.SocialImageFilename))
                siName = PrimaryTopic.SocialImageFilename;
            return siName;
        }
    }
}
