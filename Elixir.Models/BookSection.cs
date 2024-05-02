using System;

namespace Elixir.Models
{
    public class BookSection : BaseEntity
    {
        public BookSection()
        {
            
        }

        public BookSection(String name)
        {
            BookSectionName = name;
        }

        public virtual String BookSectionName { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual bool IsIncluded { get; set; }
    }
}
