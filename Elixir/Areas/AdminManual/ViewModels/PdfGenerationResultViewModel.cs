using System.Collections.Generic;
using Elixir.Models.Utils;

namespace Elixir.Areas.AdminManual.ViewModels
{
    public class PdfGenerationResultViewModel
    {
        public bool IsSuccessful { get; set; }

        public bool IsSuccessfulWithWarnings { get; set; }

        public List<string> Warnings { get; set; }

        public string Message { get; set; }

        public string NameOfFileResulted { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public bool IsSinglePageMode { get; set; }

        public bool IsSingleChapterMode { get; set; }

        public List<UpdateQueueFirstLastPage> UpdateQueueFirstLastPages { get; set; }

        public bool ShowButtonRetryAndSkipImageErrors { get; set; }

        public PdfGenerationResultViewModel()
        {
            Warnings = new List<string>();
        }
    }

    public enum ErrorCode
    {
        ErrorInPdfWriting = 1,
        ErrorInReadingDataFromDatabase = 2,
        ErrorCantOpenDatabaseFile = 3, 
    }
}
