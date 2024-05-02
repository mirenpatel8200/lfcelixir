using System;

namespace Elixir.Models
{
    public class Chapter : BaseEntity
    {
        public Chapter()
        {

        }

        public Chapter(String name)
        {
            ChapterName = name;
        }

        public virtual String ChapterName { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual bool IsIncluded { get; set; }

        public virtual String Notes { get; set; }
        public virtual String Text { get; set; }
        public virtual String ContentPage2 { get; set; }
        public virtual String ContentPage3 { get; set; }
        public virtual String ContentPage4 { get; set; }
        public virtual String ContentPage5 { get; set; }
        public virtual String ContentPage6 { get; set; }
        public virtual String ContentPage7 { get; set; }
        public virtual String ContentPage8 { get; set; }
        public virtual String ContentPage9 { get; set; }
        public virtual String ContentPage10 { get; set; }

        public virtual bool HasBreakInParagraph1 { get; set; }
        public virtual bool HasBreakInParagraph2 { get; set; }
        public virtual bool HasBreakInParagraph3 { get; set; }
        public virtual bool HasBreakInParagraph4 { get; set; }
        public virtual bool HasBreakInParagraph5 { get; set; }
        public virtual bool HasBreakInParagraph6 { get; set; }
        public virtual bool HasBreakInParagraph7 { get; set; }
        public virtual bool HasBreakInParagraph8 { get; set; }
        public virtual bool HasBreakInParagraph9 { get; set; }
        public virtual bool HasBreakInParagraph10 { get; set; }
        
        public virtual int TypeID { get; set; }

        public virtual decimal MarginTop { get; set; }

        public virtual int PageFirst { get; set; }
        public virtual int PageLast { get; set; }

    }
}
