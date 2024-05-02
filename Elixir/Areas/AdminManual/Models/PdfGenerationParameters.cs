namespace Elixir.Areas.AdminManual.Models
{
    public class PdfGenerationParameters 
    {
        public string AdditionalImagesPath { get; set; }
        
        public string FontsPath { get;  set; }

        public string FooterText { get; set; }

        public string ImagesRoot { get; set; }

        public string PdfsRoot { get; set; }

        public bool FirstSectionOnly { get; set; }

        public string PdfImagesPath { get; set; }

        public bool SkipImageExceptions { get; set; }

        public PdfGenerationParameters() { }

        public PdfGenerationParameters(string pdfsRoot, string imagesRoot, string additionalImagesPath, string fontsPath, string pdfImagesPath)
        {
            PdfsRoot = pdfsRoot;
            ImagesRoot = imagesRoot;
            AdditionalImagesPath = additionalImagesPath;
            FontsPath = fontsPath;
            PdfImagesPath = pdfImagesPath;

        }
    }
}