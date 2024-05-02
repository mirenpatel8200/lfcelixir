using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Elixir.Models;
using Elixir.Models.Validation;
using Elixir.Utils.Reflection;

namespace Elixir.Areas.Admin.Models
{
    public class AdminBlogPostModel : BlogPost
    {
        public AdminBlogPostModel(BlogPost blogPost)
        {
            ReflectionUtils.ClonePublicProperties(blogPost, this);
        }

        public AdminBlogPostModel() { }

        [Required(ErrorMessage = "Blog post title is required.")]
        public override string BlogPostTitle { get; set; }

        [Required(ErrorMessage = "URL name is required.")]
        [IsUrlName]
        public override string UrlName { get; set; }

        [AllowHtml]
        public override string ContentMain { get; set; }

        [MaxLength(255, ErrorMessage = "Notes should have a length of maximum 255 characters.")]
        public override string NotesInternal { get; set; }

        public bool IsSignificantChange { get; set; }

        [MaxLength(255, ErrorMessage = "Length should be less than 255.")]
        public override string SocialImageFilename { get; set; }
        
        [MaxLength(255, ErrorMessage = "Length should be less than 255.")]
        public override string ThumbnailImageFilename { get; set; }

        [MaxLength(255, ErrorMessage = "Blog post description should have a length of maximum 255 characters.")]
        public override string BlogPostDescriptionPublic { get; set; }

        public override string OrgsMentioned { get; set; }

        public override string OrgsMentionedString { get; set; }

        public override bool IsOrgsMentionedChanged { get; set; }
    }
}