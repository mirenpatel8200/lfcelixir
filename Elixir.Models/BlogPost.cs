using System;
using System.Collections.Generic;

namespace Elixir.Models
{
    public class BlogPost : BaseEntity
    {
        public BlogPost()
        {

        }

        public BlogPost(string title, string urlName)
        {
            BlogPostTitle = title;
            UrlName = urlName;
        }

        public virtual string BlogPostTitle { get; set; }
        public virtual string UrlName { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual string ContentMain { get; set; }

        public virtual int? PrimaryTopicId { get; set; }
        public virtual Topic PrimaryTopic { get; set; }

        public virtual int? SecondaryTopicId { get; set; }
        public virtual Topic SecondaryTopic { get; set; }

        public virtual bool IsEnabled { get; set; }

        public virtual DateTime CreatedDt { get; set; }
        public virtual DateTime? UpdatedDt { get; set; }

        public virtual string PreviousBlogPostUrlName { get; set; }
        public virtual string NextBlogPostUrlName { get; set; }

        public virtual string PreviousBlogPostTitle { get; set; }
        public virtual string NextBlogPostTitle { get; set; }

        public virtual string NotesInternal { get; set; }

        public virtual DateTime PublishedOnDT { get; set; }
        public virtual DateTime? PublishedUpdatedDT { get; set; }

        public virtual string SocialImageFilename { get; set; }
        public virtual string ThumbnailImageFilename { get; set; }

        public virtual string BlogPostDescriptionPublic { get; set; }

        public virtual string OrgsMentioned { get; set; }

        public virtual string OrgsMentionedString { get; set; }

        public virtual string PeopleMentioned { get; set; }

        public virtual string PeopleMentionedString { get; set; }

        public virtual string CreationsMentioned { get; set; }

        public virtual string CreationsMentionedString { get; set; }

        public virtual bool IsOrgsMentionedChanged { get; set; }

        public virtual bool IsPeopleMentionedChanged { get; set; }

        public virtual bool IsCreationsMentionedChanged { get; set; }

        public string[] GetHashtags()
        {
            var hashtags = new List<string>();
            var separator = new[] { " " };

            if (PrimaryTopic != null && !string.IsNullOrWhiteSpace(PrimaryTopic.Hashtags))
                hashtags.AddRange(PrimaryTopic.Hashtags.Split(separator, StringSplitOptions.RemoveEmptyEntries));
            if (SecondaryTopic != null && !string.IsNullOrWhiteSpace(SecondaryTopic.Hashtags))
                hashtags.AddRange(SecondaryTopic.Hashtags.Split(separator, StringSplitOptions.RemoveEmptyEntries));

            return hashtags.ToArray();
        }

        // <summary>
        /// Implements logic of OgImage selection.
        /// </summary>
        /// <returns></returns>
        public string GetOgImageName()
        {
            var ogImageName = "life-extension-news.jpg";
            if (!string.IsNullOrWhiteSpace(SocialImageFilename))
                ogImageName = SocialImageFilename;
            else if (PrimaryTopic != null && !string.IsNullOrWhiteSpace(PrimaryTopic.SocialImageFilenameNews))
                ogImageName = PrimaryTopic.SocialImageFilenameNews;
            else if (PrimaryTopic != null && !string.IsNullOrWhiteSpace(PrimaryTopic.SocialImageFilename))
                ogImageName = PrimaryTopic.SocialImageFilename;

            return ogImageName;
        }

        public virtual string ThumbnailImageName
        {
            get
            {
                if (!string.IsNullOrEmpty(ThumbnailImageFilename))
                    return $"/Images/{ThumbnailImageFilename}";
                else
                    return "/Images/liveforeverclub-thumb.jpg";
            }
        }
    }
}
