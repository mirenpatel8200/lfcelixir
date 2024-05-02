using System.ComponentModel.DataAnnotations;

namespace Elixir.Areas.AdminManual.Models
{
    public class PdfGenerationParametersViewModel 
    {
        [Display(Name="Footer text:")]
        public string FooterText { get; set; }

        [Display(Name ="Max 3 Body Chapters & Tips")]
        public bool FirstSectionOnly { get; set; }

        public bool SkipImageExceptions { get; set; }
    }
}