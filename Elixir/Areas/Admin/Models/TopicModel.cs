using System.ComponentModel.DataAnnotations;
using Elixir.Attributes;
using Elixir.Models;

namespace Elixir.Areas.Admin.Models
{
    public sealed class TopicModel : Topic
    {
        public TopicModel()
        {
            
        }

        public TopicModel(Topic topic)
        {
            Id = topic.Id;
            PrimaryWebPageId = topic.PrimaryWebPageId;
            PrimaryWebPageTitle = topic.PrimaryWebPage != null ? topic.PrimaryWebPage.WebPageTitle : "";
            DescriptionInternal = topic.DescriptionInternal;
            TopicName = topic.TopicName;
            NotesInternal = topic.NotesInternal;
            SocialImageFilename = topic.SocialImageFilename;
            SocialImageFilenameNews = topic.SocialImageFilenameNews;
            BannerImageFileName = topic.BannerImageFileName;
            Hashtags = topic.Hashtags;
            ThumbnailImageFilename = topic.ThumbnailImageFilename;
        }

        [Required(ErrorMessage = "Topic name field is required.")]
        [MaxLength(100, ErrorMessage = "Length should be less than 100.")]
        public override string TopicName { get; set; }

        public string PrimaryWebPageTitle { get; set; }

        [MaxLength(255, ErrorMessage = "Length should be less than 255.")]
        public override string DescriptionInternal { get; set; }
        
        public override int? PrimaryWebPageId { get; set; }

        public string PrimaryWebPage { get; set; }

        [MaxLength(255, ErrorMessage = "Notes: Length should be less than 255.")]
        public override string NotesInternal { get; set; }

        [MaxLength(255, ErrorMessage = "Banner Image: Length should be less than 255.")]
        public override string BannerImageFileName { get; set; }

        [MaxLength(255, ErrorMessage = "Hashtags: Length should be less than 255.")]
        [HashtagsTextValidator("Hashtags: Invalid format. Please include the preceeding '#', and if multiple hashtags, separate them by space.")]
        public override string Hashtags { get; set; }
    }
}