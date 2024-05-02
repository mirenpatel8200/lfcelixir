using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Elixir.Models;
using Elixir.Utils.Reflection;

namespace Elixir.Areas.AdminManual.Models
{
    public class ChapterModel : Chapter
    {
        public ChapterModel()
        {
            Notes = "";
        }

        public ChapterModel(Chapter chapter) : this()
        {
            ReflectionUtils.ClonePublicProperties(chapter, this);
            //this.Cost = bookPage.Cost.HasValue ? bookPage.Cost.Value : 0;
        }
        
        [Required(ErrorMessage = "Chapter name is required.")]
        [MaxLength(255, ErrorMessage = "Chapter name length should be maximum 255 characters.")]
        public override string ChapterName { get; set; }

        [Required(ErrorMessage = "Display Order is required.")]
        [Range(1, 999, ErrorMessage = "Display Order should be between 1 and 999.")]
        public override int DisplayOrder { get; set; }
        
        public override bool IsIncluded { get; set; }
        
        public override String Notes { get; set; }

        public override int TypeID { get; set; }

        [MaxLength(10000, ErrorMessage = "Text lenght should be maximum 10.000 characters.")]
        [AllowHtml]
        public override string Text { get; set; }

        [MaxLength(10000, ErrorMessage = "Content Page 2 length should be maximum 10.000 characters.")]
        [AllowHtml]
        public override string ContentPage2 { get; set; }

        [MaxLength(10000, ErrorMessage = "Content Page 3 length should be maximum 10.000 characters.")]
        [AllowHtml]
        public override string ContentPage3 { get; set; }

        [MaxLength(10000, ErrorMessage = "Content Page 4 length should be maximum 10.000 characters.")]
        [AllowHtml]
        public override string ContentPage4 { get; set; }

        [MaxLength(10000, ErrorMessage = "Content Page 5 length should be maximum 10.000 characters.")]
        [AllowHtml]
        public override string ContentPage5 { get; set; }

        [MaxLength(10000, ErrorMessage = "Content Page 6 length should be maximum 10.000 characters.")]
        [AllowHtml]
        public override string ContentPage6 { get; set; }

        [MaxLength(10000, ErrorMessage = "Content Page 7 length should be maximum 10.000 characters.")]
        [AllowHtml]
        public override string ContentPage7 { get; set; }

        [MaxLength(10000, ErrorMessage = "Content Page 8 length should be maximum 10.000 characters.")]
        [AllowHtml]
        public override string ContentPage8 { get; set; }

        [MaxLength(10000, ErrorMessage = "Content Page 9 length should be maximum 10.000 characters.")]
        [AllowHtml]
        public override string ContentPage9 { get; set; }

        [MaxLength(10000, ErrorMessage = "Content Page 10 length should be maximum 10.000 characters.")]
        [AllowHtml]
        public override string ContentPage10 { get; set; }
        
        [Required(ErrorMessage = "Top Margin is Required")]
        [Range(0, 9.99, ErrorMessage = "Top Margin should be between 0 and 9.99")]
        public override decimal MarginTop { get; set; }

        public Dictionary<int, string> ChapterTypes { get; set; }

        public override bool HasBreakInParagraph1 { get; set; }

        public override bool HasBreakInParagraph2 { get; set; }

        public override bool HasBreakInParagraph3 { get; set; }

        public override bool HasBreakInParagraph4 { get; set; }

        public override bool HasBreakInParagraph5 { get; set; }

        public override bool HasBreakInParagraph6 { get; set; }

        public override bool HasBreakInParagraph7 { get; set; }

        public override bool HasBreakInParagraph8 { get; set; }

        public override bool HasBreakInParagraph9 { get; set; }

        public override bool HasBreakInParagraph10 { get; set; }
    }
}