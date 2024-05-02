using System;
using System.Collections.Generic;
using System.Text;
using Elixir.Utils;

namespace Elixir.Models
{
    public class Article : BaseEntity
    {
        public Article()
        {

        }

        public Article(string title, string urlName, DateTime publishedDate)
        {
            Title = title;
            UrlName = urlName;
            ArticleDate = publishedDate;
        }

        public virtual string Title { get; set; }
        public virtual string OriginalTitle { get; set; }

        public virtual string DnPublisherName { get; set; }
        public virtual string PublisherUrl { get; set; }

        public virtual string DnReporterName { get; set; }

        public virtual DateTime ArticleDate { get; set; }

        public virtual Topic PrimaryTopic { get; set; }
        public virtual Topic SecondaryTopic { get; set; }

        public virtual int PrimaryTopicID { get; set; }
        public virtual int? SecondaryTopicID { get; set; }

        public virtual string BulletPoints { get; set; }
        public virtual string Summary { get; set; }

        public virtual bool IsEnabled { get; set; }

        public virtual bool IsDeleted { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime? Updated { get; set; }

        public virtual string Notes { get; set; }
        public virtual string UrlName { get; set; }

        public virtual string IdHashCode { get; set; }

        public virtual string SocialImageFilename { get; set; }

        public virtual int? PublisherResourceId { get; set; }
        public virtual int? ReporterResourceId { get; set; }

        public virtual bool DisplaySocialImage { get; set; }

        public virtual bool IsHumour { get; set; }

        public virtual int? CreatedByUserId { get; set; }
        public virtual int? UpdatedByUserId { get; set; }

        public virtual string LastUpdatedBy { get; set; }

        public virtual string OrgsMentioned { get; set; }

        public virtual string OrgsMentionedString { get; set; }

        public virtual string PeopleMentioned { get; set; }

        public virtual string PeopleMentionedString { get; set; }

        public virtual string CreationsMentioned { get; set; }

        public virtual string CreationsMentionedString { get; set; }

        public virtual bool IsOrgsMentionedChanged { get; set; }

        public virtual bool IsPeopleMentionedChanged { get; set; }

        public virtual bool IsCreationsMentionedChanged { get; set; }
        /// <summary>
        /// Calculates hash code basing on ArticleDate and Id. Hash collisions may happen.
        /// </summary>
        /// <returns></returns>
        public string CalculateIdHashCode()
        {
            var date = Created;
            var yearSub = date.Year - 2000;
            var year = yearSub >= 0 ? yearSub : 0;
            var yy = year.ToSimple2CharsBase26();
            var dd = (31 * (date.Month - 1) + date.Day).ToSimple2CharsBase26();
            var h = date.Hour.ToABasedLetter();
            var m = (date.Minute / 3).ToABasedLetter();
            var s = (date.Second / 3).ToABasedLetter();

            var sId = Id.Value.ToString("D3");
            var nnn = sId.Length > 3 ? sId.Substring(sId.Length - 3) : sId;

            var sb = new StringBuilder();
            sb.Append(yy).Append(dd).Append(h).Append(m).Append(s).Append(nnn);

            return sb.ToString();
        }

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

        /// <summary>
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

    }
}
