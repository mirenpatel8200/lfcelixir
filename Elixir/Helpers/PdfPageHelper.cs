using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using static Elixir.Models.Utils.AppConstants;

namespace Elixir.Helpers
{
    //Class used for page-level events, used mainly for page-repeated things
    //e.g. colored blocks for tip pages, the orange/blue header, page number etc.  
    public class PdfPageHelper : PdfPageEventHelper
    {
        #region Properties

        public string Footer { get; set; }

        //TODO: delete when Adrian approves page nr. positions
        public float PageX { get; set; }

        public float PageY { get; set; }

        public float OrangeX { get; set; }

        public float OrangeY { get; set; }

        public float OrangeWidth { get; set; }

        public float BlueX { get; set; }

        public float BlueY { get; set; }

        public float BlueWidth { get; set; }

        public float TipsX { get; set; }

        public float TipsY { get; set; }
        
        public float BeigeX { get; set; }

        public float BeigeY { get; set; }

        public float LightBlueX { get; set; }

        public float LightBlueY { get; set; }

        public float LightBlueWidth { get; set; }

        public bool ShowTipsBackgroundsFromThisPageOn { get; set; }

        public bool ShowHeaderAndPageNumberFromThisPageOn { get; set; }
        
        public BaseFont WritingFont { get; set; }

        public string ImagesRootPath { get; set; }
        
        public float ChevronX { get; set; }

        public float ChevronY { get; set; }
        
        public bool ForceEvenPage { get; set; }
        #endregion

        public PdfPageHelper(string footer, string imagesRootPath)
        {
            this.Footer = footer;
            this.ImagesRootPath = imagesRootPath;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfContentByte canvas = writer.DirectContentUnder;

            #region Setup Variables independent of page number
            
            PageY = document.PageSize.Height - document.TopMargin - 9;
            BeigeX = 0;
            BeigeY = document.PageSize.Height -
                PdfGen.HeaderHeight -
                0.03f * PdfGen.Inch -
                PdfGen.ScoreBarsBackgroundHeight;
            LightBlueX = document.LeftMargin;
            LightBlueY = document.BottomMargin;
            TipsY = document.PageSize.Height - (4.34f * PdfGen.Inch) - PdfGen.TipsHeight;
            
            #endregion

            #region Setup Variables that depend on page number Odd/Even
            if (writer.PageNumber % 2 > 0 && !this.ForceEvenPage)
            {
                //TODO: delete when Adrian approves page nr. positions
                //PageX = document.Right + PdfGen.Adjustment02 - 10;
                //if (writer.PageNumber >= 10)
                //    PageX = document.Right + PdfGen.Adjustment02 - 13;
                //if (writer.PageNumber >= 100)
                //    PageX = document.Right + PdfGen.Adjustment02 - 18; // adjusted 181015 (was -16) - KDP checked failed for out of safe zone

                OrangeX = 0;
                OrangeY = document.PageSize.Height - PdfGen.HeaderHeight;

                BlueX = document.PageSize.Width * PdfGen.HeaderTextBlockWidthPercent + PdfGen.HeaderSpaceBetweenTextAndPageNumberBlocks;
                BlueY = document.PageSize.Height - PdfGen.HeaderHeight;

                OrangeWidth = document.PageSize.Width * PdfGen.HeaderTextBlockWidthPercent;
                BlueWidth = document.PageSize.Width * PdfGen.HeaderPageNumberWidthPercent;

                TipsX = document.PageSize.Width - PdfGen.TipsWidth;
                
                ChevronX = document.PageSize.Width - PdfGen.ChevronImageWidth;
                ChevronY = TipsY;
            }
            else
            {
                //TODO: delete when Adrian approves page nr. positions
                //PageX = document.Left + 6 - PdfGen.Adjustment02;
                //if (writer.PageNumber >= 10)
                //    PageX = document.Left + 4 - PdfGen.Adjustment02;
                //if (writer.PageNumber >= 100)
                //    PageX = document.Left + 2 - PdfGen.Adjustment02;

                OrangeX = document.PageSize.Width * PdfGen.HeaderPageNumberWidthPercent + PdfGen.HeaderSpaceBetweenTextAndPageNumberBlocks;
                OrangeY = document.PageSize.Height - PdfGen.HeaderHeight;

                BlueX = 0;
                BlueY = document.PageSize.Height - PdfGen.HeaderHeight;

                OrangeWidth = document.PageSize.Width * PdfGen.HeaderTextBlockWidthPercent;
                BlueWidth = document.PageSize.Width * PdfGen.HeaderPageNumberWidthPercent;

                TipsX = 0;
                
                ChevronX = TipsX;
                ChevronY = TipsY;
            }

            #endregion

            #region Page Number
            if (ShowHeaderAndPageNumberFromThisPageOn)
            {
                Phrase pageNumber = new Phrase(writer.PageNumber.ToString(),
                    new Font(this.WritingFont, 12, Font.BOLD, PdfGen.White));
                
                if (writer.PageNumber % 2 > 0 && !this.ForceEvenPage)
                    ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT, pageNumber, (document.Right + PdfGen.Adjustment02), PageY, 0);
                else
                    ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, pageNumber, (document.Left - PdfGen.Adjustment02), PageY, 0);
            }
            
            #endregion

            #region Orange rectangle - Header
            if (ShowHeaderAndPageNumberFromThisPageOn)
            {
                canvas.SetColorStroke(PdfGen.Orange);
                canvas.SetColorFill(PdfGen.Orange);

                canvas.Rectangle(
                    OrangeX,
                    OrangeY,
                    OrangeWidth,
                    PdfGen.HeaderHeight);

                canvas.Fill();
            }
            #endregion

            #region Blue rectangle - Header
            if (ShowHeaderAndPageNumberFromThisPageOn)
            {
                canvas.SetColorStroke(PdfGen.Blue);
                canvas.SetColorFill(PdfGen.Blue);

                canvas.Rectangle(
                    BlueX,
                    BlueY,
                    BlueWidth,
                    PdfGen.HeaderHeight);

                canvas.Fill();
            }
            #endregion

            #region Beige rectangle - score bars background
            if (ShowTipsBackgroundsFromThisPageOn)
            {
                canvas.SetColorStroke(PdfGen.Beige);
                canvas.SetColorFill(PdfGen.Beige);

                canvas.Rectangle(
                    BeigeX,
                    BeigeY,
                    document.PageSize.Width,
                    PdfGen.ScoreBarsBackgroundHeight);
                canvas.Fill();
            }

            #endregion

            #region Light blue rectangle - research papers background
            if (ShowTipsBackgroundsFromThisPageOn)
            { 
                canvas.SetColorStroke(PdfGen.LightBlue);
                canvas.SetColorFill(PdfGen.LightBlue);

                canvas.Rectangle(
                    LightBlueX,
                    LightBlueY,
                    PdfGen.SafeZoneWidth,
                    PdfGen.ResearchPapersBoxHeight);

                canvas.Fill();
            }
            #endregion

            #region Beige rectangle - tips section background
            if (ShowTipsBackgroundsFromThisPageOn)
            {
                canvas.SetColorStroke(PdfGen.Beige);
                canvas.SetColorFill(PdfGen.Beige);

                canvas.Rectangle(
                    TipsX,
                    TipsY,
                    PdfGen.TipsWidth,
                    PdfGen.TipsHeight);

                canvas.Fill();

                string fullChevronPath = (writer.PageNumber % 2 > 0 && !this.ForceEvenPage) ?
                    Path.Combine(this.ImagesRootPath, PdfGen.ChevronOddPage) :
                    Path.Combine(this.ImagesRootPath, PdfGen.ChevronEvenPage);

                var chevron = iTextSharp.text.Image.GetInstance(fullChevronPath);
                chevron.ScalePercent(24f);
                chevron.SetAbsolutePosition(ChevronX, ChevronY);
                document.Add(chevron);
            }
            
            #endregion

            #region Footer (user inserted)

            if (!string.IsNullOrEmpty(this.Footer))
            {
                Font footerFont = new Font(Font.FontFamily.UNDEFINED, 10, Font.ITALIC, new BaseColor(128, 128, 128));
                PdfContentByte contentByte = writer.DirectContent;
                Phrase footer = new Phrase(this.Footer, footerFont);

                ColumnText.ShowTextAligned(
                    writer.DirectContent,
                    Element.ALIGN_CENTER,
                    footer,
                    (document.Right - document.Left) / 2 + document.LeftMargin,
                    document.Bottom + 10,
                    0);
            }

            #endregion

            #region Footer - fixed

            //Font normal = new Font(FontFamily.UNDEFINED, 10, Font.NORMAL, new BaseColor(0, 154, 220));
            //Font bold = new Font(FontFamily.UNDEFINED, 10, Font.BOLD, new BaseColor(0, 154, 220));

            //Chunk footer1 = new Chunk(Constants.FooterFixedPart1, bold);
            //Chunk footer2 = new Chunk(Constants.FooterFixedPart2, normal);

            //Paragraph footerFixed = new Paragraph();
            //footerFixed.Add(footer1);
            //footerFixed.Add(footer2);

            //ColumnText.ShowTextAligned(
            //    writer.DirectContent,
            //    Element.ALIGN_CENTER,
            //    footerFixed,
            //    (document.Right - document.Left) / 2 + document.LeftMargin,
            //    document.Bottom, 
            //    0);

            #endregion
            
        }
    }
}