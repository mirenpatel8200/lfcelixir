using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Elixir.Models;
using Elixir.Utils.Reflection;

namespace Elixir.Areas.AdminManual.Models
{
    public sealed class BookPageModel : BookPage
    {
        public BookPageModel()
        {
            Notes = "";
        }

        public BookPageModel(BookPage bookPage) : this()
        {
            ReflectionUtils.ClonePublicProperties(bookPage, this);

            this.Cost = bookPage.Cost.HasValue ? bookPage.Cost.Value : 0;
            this.Difficulty = bookPage.Difficulty.HasValue ? bookPage.Difficulty.Value : 0;
        }

        [Required(ErrorMessage = "Section is required.")]
        public int BookSectionId { get; set; }
        
        [Required(ErrorMessage = "Section name is required.")]
        [MaxLength(256, ErrorMessage = "Length should be less than 256.")]
        public override string BookPageName { get; set; }
        
        [Range(1, 99, ErrorMessage = "Order (within selection) should be between 1 and 99.")]
        public override int DisplayOrder { get; set; }

        [Range(0, 9999, ErrorMessage = "Life Extension should be between 0 and 9999.")]
        public override int LifeExtension40 { get; set; }

        [MaxLength(20, ErrorMessage = "Length should be less than 20.")]
        public override string Status { get; set; }

        [MaxLength(20, ErrorMessage = "Length should be less than 20.")]
        public override string Author { get; set; }

        [Required(ErrorMessage = "Cost is required.")]
        [Range(0, 10, ErrorMessage = "Cost must be between 0 and 10")]
        public override int? Cost { get; set; }

        [MaxLength(255, ErrorMessage = "Image Filename must have maximum 255 characters.")]
        public override string ImageFilename { get; set; }

        [Required(ErrorMessage = "Difficulty is required.")]
        [Range(0, 10, ErrorMessage ="Difficulty must be between 0-10")]
        public override int? Difficulty { get; set; }

        [MaxLength(2000, ErrorMessage = "Resources must have maximum 2000 characters.")]
        [AllowHtml]
        public override string Resources { get; set; }

        [MaxLength(1000, ErrorMessage = "Tips must have maximum 1000 characters.")]
        public override string Tips { get; set; }

        [MaxLength(2000, ErrorMessage = "Research Papers must have maximum 2000 characters.")]
        [AllowHtml]
        public override string ResearchPapers { get; set; }

        [AllowHtml]
        [MaxLength(10000, ErrorMessage = "Description should have maximum 10000 characters.")]
        public override string BookPageDescription { get; set; }

        [MaxLength(255, ErrorMessage = "Notes should have maximum 255 characters.")]
        public override string Notes { get; set; }

    }
}