using System.ComponentModel.DataAnnotations;
using Elixir.Models;

namespace Elixir.Areas.AdminManual.Models
{
    public sealed class BookSectionModel : BookSection
    {
        public BookSectionModel()
        {
            
        }
        
        public BookSectionModel(BookSection bookSection)
        {
            Id = bookSection.Id;
            BookSectionName = bookSection.BookSectionName;
            DisplayOrder = bookSection.DisplayOrder;
            IsIncluded = bookSection.IsIncluded;
        }

        [Required(ErrorMessage = "Section name is required.")]
        [StringLength(256, ErrorMessage = "Length should be less than 256.")]
        public override string BookSectionName { get; set; }

        [Range(1, 99, ErrorMessage = "DisplayOrder should be between 1 and 99.")]
        public override int DisplayOrder { get; set; }
    }
}