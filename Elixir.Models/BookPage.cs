using System;

namespace Elixir.Models
{
    public class BookPage : BaseEntity
    {
        public BookPage()
        {

        }

        public BookPage(String name)
        {
            BookPageName = name;
        }

        public virtual String BookPageName { get; set; }
        public virtual String BookPageDescription { get; set; }
        public virtual String ImageFilename { get; set; }
        public virtual int? BookSectionId { get; set; }
        public virtual BookSection BookSection { get; set; }

        public virtual int DisplayOrder { get; set; }
        public virtual int LifeExtension40 { get; set; }
        public virtual int? Difficulty { get; set; }

        public virtual bool IsIncluded { get; set; }
        public virtual String Notes { get; set; }

        public virtual String Status { get; set; }
        public virtual String Author { get; set; }
        public virtual int? Cost { get; set; }

        public virtual string Resources { get; set; }
        public virtual string Tips { get; set; }
        public virtual string ResearchPapers { get; set; }
        public virtual int? PageFirst { get; set; }
        public virtual int? PageLast { get; set; }
    }
}
