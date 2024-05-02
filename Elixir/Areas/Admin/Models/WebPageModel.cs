using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Elixir.Models;
using Elixir.Utils.Reflection;
using Elixir.Models.Validation;

namespace Elixir.Areas.Admin.Models
{
    public class WebPageModel : WebPage
    {
        // TODO: AY - move ErrorMessages to Constants.

        public WebPageModel() { }
        public WebPageModel(WebPage webPage) : this()
        {
            ReflectionUtils.ClonePublicProperties(webPage, this);
        }

        [Required(ErrorMessage = "UrlName is required.")]
        [IsUrlName]
        public override string UrlName { get; set; }

        [Required(ErrorMessage = "WebPageName is required.")]
        public override string WebPageName { get; set; }

        public override bool IsDeleted { get; set; }

        [Required(ErrorMessage = "WebPageTitle is required.")]
        public override string WebPageTitle { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "ContentMain is required.")]
        public override string ContentMain { get; set; }

        public override int? ParentID { get; set; }

        [Required(ErrorMessage = "IsSubjectPage is required.")]
        public override bool IsSubjectPage { get; set; }
        
        [Required(ErrorMessage = "Display Order is required.")]
        [Range(0, 99, ErrorMessage = "Display Order must be between 0-99")]
        public override int DisplayOrder { get; set; }

        public override string NotesInternal { get; set; }

        [MaxLength(255, ErrorMessage = "Social Image Filename should have a length of maximum 255 characters.")]
        public override string SocialImageFileName { get; set; }

        [MaxLength(255, ErrorMessage = "Meta Description should have a length of maximum 255 characters.")]
        public override string MetaDescription { get; set; }

        [Required(ErrorMessage = "Web Page Type is Required")]
        public override int TypeID { get; set; }
    }
}