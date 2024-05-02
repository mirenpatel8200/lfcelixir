
using System.Collections.Generic;

namespace Elixir.Areas.Admin.Models
{
    public class TextOnImageModel
    {
        public string ImageSource { get; set; }

        public string ImageResultSource { get; set; }

        public string Text { get; set; }

        public List<string> FontsAvailable { get; set; }
        
        public List<string> TextColorsAvailable { get; set; }

        public string SelectedFont { get; set; }

        public int SelectedSize { get; set; }

        public string SelectedColor { get; set; }

        public bool RenderImage { get;  set; }

        public string GeneratedImageFilename { get; set; }
        
        public TextOnImageModel()
        {
            FontsAvailable = new List<string>();
            TextColorsAvailable = new List<string>();
        }
    }
}