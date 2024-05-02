using System;

namespace Elixir.Models
{
    public class Topic : BaseEntity
    {
        public Topic()
        {
            
        }

        public Topic(String topicName, String descriptionInternal)
        {
            TopicName = topicName;
            DescriptionInternal = descriptionInternal;
        }

        public virtual WebPage PrimaryWebPage { get; set; }
        public virtual int? PrimaryWebPageId { get; set; }

        public virtual string TopicName { get; set; }
        public virtual string DescriptionInternal { get; set; }
        public virtual string NotesInternal { get; set; }
        public virtual string SocialImageFilename { get; set; }
        public virtual string SocialImageFilenameNews { get; set; }
        public virtual string BannerImageFileName { get; set; }
        public virtual string Hashtags { get; set; }
        public virtual string ThumbnailImageFilename { get; set; }
    }
}
