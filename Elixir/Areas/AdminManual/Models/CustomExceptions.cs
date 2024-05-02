using System;

namespace Elixir.Areas.AdminManual.Models
{
    public class ImageNotFoundException : Exception
    {
        public string MissingFile { get; set; }

        public ImageNotFoundException(string fullerror) : base(fullerror)
        {
            int filenameStart = fullerror.LastIndexOf("\\") + 1;
            int filenameEnd = fullerror.LastIndexOf("'");
            string filename = fullerror.Substring(filenameStart, filenameEnd - filenameStart);
            MissingFile = filename;
        }
    }
    
}