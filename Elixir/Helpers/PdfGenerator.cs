using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elixir.Areas.AdminManual.Models;
using Elixir.Areas.AdminManual.ViewModels;
using Elixir.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Chapter = Elixir.Models.Chapter;
using Elixir.Models.Utils;
using iTextSharp.text.pdf.draw;
using pdf = iTextSharp.text;
using static Elixir.Models.Utils.AppConstants;
using iTextSharp.text.pdf.interfaces;

namespace Elixir.Helpers
{
    // BUG: AY - should be moved to BusinessLogic project - important! Otherwise - dependency violation.
    public class PdfGenerator
    {
        public PdfGenerationParameters Parameters { get; set; }

        public static BaseFont Goudy { get; set; }

        public static BaseFont Helvetica { get; set; }

        Dictionary<string, pdf.Image> DotsImages { get; set; }


        public PdfGenerator(PdfGenerationParameters parameters)
        {
            Parameters = parameters;
        }

        private void LoadFonts(string fontsPath)
        {
            Goudy = BaseFont.CreateFont(
                Path.Combine(fontsPath, PdfGen.Goudy),
                BaseFont.WINANSI, BaseFont.EMBEDDED);

            Helvetica = BaseFont.CreateFont(
                Path.Combine(fontsPath, PdfGen.Helvetica),
                BaseFont.WINANSI, BaseFont.EMBEDDED);
        }

        public PdfGenerationResultViewModel GeneratePdfFile(
            List<BookPage> pages,
            List<Chapter> chaptersFront,
            List<Chapter> chaptersBody,
            List<Chapter> chaptersBack,
            List<BookSection> sections)
        {
            PdfGenerationResultViewModel result = new PdfGenerationResultViewModel();

            LoadFonts(Parameters.FontsPath);

            #region Initialization - Setup Page Size + Margins
            Rectangle pageSize = new Rectangle(PdfGen.PageWidth, PdfGen.PageHeight);

            Document document = new Document(pageSize,
                PdfGen.MarginLeft, PdfGen.MarginRight,
                PdfGen.MarginTop, PdfGen.MarginBottomForText);

            // ebook
            StreamWriter ebookFile = null;
            StreamWriter debugLogFile = null;

            #endregion

            try
            {
                #region Setup Global variables
                PdfUtils.ImagesPath = Parameters.PdfImagesPath;

                List<UpdateQueueFirstLastPage> updateQueue = new List<UpdateQueueFirstLastPage>();

                string fileName = $"Manual_{ DateTime.Now.Date.ToString("yyyyMMdd")}-{DateTime.Now.ToString("HHmm")}.pdf";

                string fullOutputPath = Path.Combine(Parameters.PdfsRoot, fileName);
                pdf.Image image;

                DotsImages = PdfUtils.GetDotsImages(Parameters.AdditionalImagesPath);

                pdf.Image[] imagesCost = new pdf.Image[5];
                pdf.Image[] imagesDifficulty = new pdf.Image[5];
                pdf.Image[] imagesImpact = new pdf.Image[5];
                Paragraph emptyParagraph = new Paragraph(" ");
                float sectionTableX;
                Paragraph pageTitle;

                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(fullOutputPath, FileMode.Create));
                PdfPageHelper pageHelper = new PdfPageHelper(Parameters.FooterText, Parameters.AdditionalImagesPath);
                pageHelper.ShowTipsBackgroundsFromThisPageOn = false;
                pageHelper.WritingFont = Helvetica;

                writer.PageEvent = pageHelper;

                document.SetMarginMirroring(true);

                bool[] ChapterPageBreaksInParagraph = new bool[11]; // matches Page and allows check for previous page break on first page without error
                #endregion

                document.Open();
                // ebook
                fullOutputPath = Path.Combine(Parameters.PdfsRoot, EBook.EbookOutputFile);
                ebookFile = new System.IO.StreamWriter(fullOutputPath);
                ebookFile.WriteLine(EBook.EbookHtmlHeader);
                debugLogFile = new System.IO.StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: false);
                debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Start PDF gen");
                debugLogFile.Close();

                #region Chapters - Front
                chaptersFront = chaptersFront.OrderBy(c => c.DisplayOrder).ToList();
                ColumnText ct = new ColumnText(writer.DirectContent);
                Rectangle size;
                Paragraph chapterText; List<IAccessibleElement> chapterRichText;
                Paragraph tipText; List<IAccessibleElement> tipRichText;
                Paragraph researchpaperText; List<IAccessibleElement> researchpaperRichText;

                for (int cf = 0; cf < chaptersFront.Count; cf++)
                {
                    List<string> chapterPages = PdfUtils.GetAllChapterPages(chaptersFront[cf]);
                    #region UQ
                    updateQueue.Add(
                        new UpdateQueueFirstLastPage
                        {
                            EntityId = chaptersFront[cf].Id.Value,
                            Type = PdfPageType.FrontChapter,
                            PageFirst = writer.PageNumber,
                            PageLast = writer.PageNumber + chapterPages.Count - 1
                        });
                    #endregion
                    string nameString = chaptersFront[cf].ChapterName;

                    // ebook
                    ebookFile.WriteLine(EBook.EbookPageBreak);
                    ebookFile.WriteLine("\r\n<div id=\"chapter-" + chaptersFront[cf].Id + "\"></div>");

                    for (int chapterPage = 0; chapterPage < chapterPages.Count; chapterPage++)
                    {
                        HtmlComponentList elements = HtmlUtils.ExtractHTMLElements(PdfUtils.RemoveKindleHtml(chapterPages[chapterPage]));

                        //Fit text in rectangle - MarginTop is Dynamic, per chapter
                        // 181015 AC mirror margins don't change left/right margin values - have to do manually - even pages rejected by KDP as text to far to the right                   

                        if (writer.PageNumber % 2 > 0)
                            size = new Rectangle(
                            document.LeftMargin,
                            PdfGen.ResearchPapersBoxHeight + document.BottomMargin,
                            document.PageSize.Width - document.RightMargin,
                            document.PageSize.Height - document.TopMargin -
                            ((float)chaptersFront[cf].MarginTop * PdfGen.Inch));
                        else
                            size = new Rectangle(
                            document.RightMargin,
                            PdfGen.ResearchPapersBoxHeight + document.BottomMargin,
                            document.PageSize.Width - document.LeftMargin,
                            document.PageSize.Height - document.TopMargin -
                            ((float)chaptersFront[cf].MarginTop * PdfGen.Inch));

                        ct.SetSimpleColumn(size);

                        if (elements.JustPlainText)
                        {
                            if (!string.IsNullOrEmpty(chapterPages[chapterPage]))
                            {
                                chapterText = new Paragraph(PdfGen.LeadingNormalText,
                                    chapterPages[chapterPage], new Font(Helvetica, 9));
                                chapterText.SpacingAfter = 4;
                                ct.AddElement(chapterText);
                            }
                        }
                        else
                        {
                            chapterRichText = PdfUtils.GetRichTextForPdf(elements, Helvetica, 9, PdfGen.LeadingNormalText, 4, Parameters.SkipImageExceptions);

                            FillColumnWithRichText(ref ct, chapterRichText);

                        }
                        ct.Go();

                        document.NewPage();

                        // ebook
                        ebookFile.WriteLine(PdfUtils.GetKindleHtml(PdfUtils.GetHTMLSafeCharacters(chapterPages[chapterPage])));
                    }
                }
                debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done front matter");
                debugLogFile.Close();
                #endregion

                pageHelper.ShowHeaderAndPageNumberFromThisPageOn = true;

                #region Table of Contents

                List<TableOfContentItem> tableOfContentsItems = PdfUtils.
                    GetTableOfContentsItems(chaptersFront, chaptersBody, sections, pages);

                #region TOC Header + Title
                Paragraph header = new Paragraph(
                    PdfGen.TableOfContentsHeader,
                    new Font(Helvetica, 12, Font.BOLD, PdfGen.White));
                header.Alignment = Element.ALIGN_CENTER;

                PdfPTable tableForHeader = PdfUtils.GetTable(header, PdfGen.HeaderTextBlockWidth);

                float headerTableX = (writer.PageNumber % 2 > 0) ? 0 :
                        PdfGen.HeaderPageNumberBlockWidth + PdfGen.HeaderSpaceBetweenTextAndPageNumberBlocks;

                tableForHeader.WriteSelectedRows(0, -1,
                    headerTableX,
                    document.PageSize.Height - document.TopMargin + 2,
                    writer.DirectContent);

                Paragraph title = new Paragraph(
                    PdfGen.TableOfContentsTitle,
                    new Font(Helvetica, 12, Font.BOLD));
                title.SpacingBefore = 2;
                title.SpacingAfter = 2;
                title.Alignment = Element.ALIGN_LEFT;

                document.Add(new Paragraph(" "));
                document.Add(title);
                document.Add(Chunk.NEWLINE);
                // ebook
                ebookFile.WriteLine(EBook.EbookPageBreak);
                ebookFile.WriteLine("\r\n<h2 id=\"TOC\" class=\"pageheader\">" + PdfGen.TableOfContentsTitle + "</h2></div>");
                ebookFile.WriteLine("\r\n<nav epub:type=\"toc\"><ul class=\"toclist\">");
                #endregion

                Paragraph tocLine;
                Font tableOfContentFont = new Font(Helvetica, 10);
                Chunk dottedLine = new Chunk(new DottedLineSeparator());
                int linesPerpage = 0;
                for (int tocItem = 1; tocItem < tableOfContentsItems.Count; tocItem++)
                {
                    var contentItem = tableOfContentsItems[tocItem];

                    if (contentItem.Type == PdfPageType.FrontChapter ||
                        contentItem.Type == PdfPageType.TableOfContents)
                    {
                        continue;
                    }

                    else if (contentItem.Type == PdfPageType.BodyChapter)
                    {
                        tocLine = new Paragraph(12, contentItem.DisplayName, tableOfContentFont);
                        tocLine.Add(dottedLine);
                        tocLine.Add(new Chunk(contentItem.PageNumber.ToString()));
                        document.Add(tocLine);
                        // ebook
                        ebookFile.WriteLine("\r\n<li><a href=\"#chapter-" + contentItem.DatabaseId.ToString() + "\">" + contentItem.DisplayName + "</a></li>");

                        linesPerpage++;
                    }

                    else if (contentItem.Type == PdfPageType.Section)
                    {
                        tocLine = new Paragraph(12, contentItem.DisplayName.ToUpper(), tableOfContentFont);
                        document.Add(tocLine);
                        // ebook
                        ebookFile.WriteLine("\r\n<li>" + contentItem.DisplayName.ToUpper() + "</li>");

                        linesPerpage++;
                    }

                    else if (contentItem.Type == PdfPageType.Tip)
                    {
                        tocLine = new Paragraph(12, contentItem.DisplayName, tableOfContentFont);
                        tocLine.Add(dottedLine);
                        tocLine.Add(contentItem.PageNumber.ToString());
                        document.Add(tocLine);
                        // ebook
                        ebookFile.WriteLine("\r\n<li><a href=\"#tip-" + contentItem.DatabaseId.ToString() + "\">" + contentItem.DisplayName + "</a></li>");

                        linesPerpage++;
                    }

                    if (linesPerpage % PdfGen.TableOfContentsLinesPerPage == 0)
                    {
                        tocLine = new Paragraph();
                        document.NewPage();

                        tableForHeader.WriteSelectedRows(0, -1,
                            (writer.PageNumber % 2 > 0) ? 0 :
                                PdfGen.HeaderPageNumberBlockWidth + PdfGen.HeaderSpaceBetweenTextAndPageNumberBlocks,
                            document.PageSize.Height - document.TopMargin + 2,
                            writer.DirectContent);

                        document.Add(new Paragraph(" "));
                        document.Add(title);
                        document.Add(Chunk.NEWLINE);
                    }

                }
                // ebook
                ebookFile.WriteLine("</ul>");

                document.NewPage();

                debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done table of contents");
                debugLogFile.Close();
                #endregion

                #region Chapters - Body
                Paragraph chapterHeader;
                PdfPTable tableHeaderName;
                Paragraph chapterName, space;

                int CountChapterPagesGenerated = 0;
                chaptersBody = chaptersBody.OrderBy(c => c.DisplayOrder).ToList();
                for (int cb = 0; cb < chaptersBody.Count; cb++)
                {
                    // ebook
                    ChapterPageBreaksInParagraph[0] = false;
                    ChapterPageBreaksInParagraph[1] = chaptersBody[cb].HasBreakInParagraph1;
                    ChapterPageBreaksInParagraph[2] = chaptersBody[cb].HasBreakInParagraph2;
                    ChapterPageBreaksInParagraph[3] = chaptersBody[cb].HasBreakInParagraph3;
                    ChapterPageBreaksInParagraph[4] = chaptersBody[cb].HasBreakInParagraph4;
                    ChapterPageBreaksInParagraph[5] = chaptersBody[cb].HasBreakInParagraph5;
                    ChapterPageBreaksInParagraph[6] = chaptersBody[cb].HasBreakInParagraph6;
                    ChapterPageBreaksInParagraph[7] = chaptersBody[cb].HasBreakInParagraph7;
                    ChapterPageBreaksInParagraph[8] = chaptersBody[cb].HasBreakInParagraph8;
                    ChapterPageBreaksInParagraph[9] = chaptersBody[cb].HasBreakInParagraph9;
                    ChapterPageBreaksInParagraph[10] = chaptersBody[cb].HasBreakInParagraph10;
                    ebookFile.WriteLine(EBook.EbookPageBreak);
                    // ebookFile.WriteLine("\r\n<div class=\"pageheader\"><p class=\"pageheader\">" + chaptersBody[cb].ChapterName.ToUpper() + "</p></div>");
                    ebookFile.WriteLine("\r\n<h2 id=\"chapter-" + chaptersBody[cb].Id.ToString() + "\" class=\"pageheader\">" + chaptersBody[cb].ChapterName.ToUpper() + "</h2>");
                    ebookFile.WriteLine("<h1>" + chaptersBody[cb].ChapterName + "</h1>");

                    List<string> chapterPages = PdfUtils.GetAllChapterPages(chaptersBody[cb]);
                    #region UQ
                    updateQueue.Add(
                        new UpdateQueueFirstLastPage
                        {
                            EntityId = chaptersBody[cb].Id.Value,
                            Type = PdfPageType.BodyChapter,
                            PageFirst = writer.PageNumber,
                            PageLast = writer.PageNumber + chapterPages.Count - 1
                        });
                    #endregion
                    for (int chapterPage = 0; chapterPage < chapterPages.Count; chapterPage++)
                    {
                        string nameString = chaptersBody[cb].ChapterName;

                        #region Header text

                        chapterHeader = new Paragraph(chaptersBody[cb].ChapterName.ToUpper(),
                            new Font(Helvetica, 12, Font.BOLD, PdfGen.White));
                        chapterHeader.Alignment = Element.ALIGN_CENTER;

                        headerTableX = (writer.PageNumber % 2 > 0) ? 0 :
                        PdfGen.HeaderPageNumberBlockWidth + PdfGen.HeaderSpaceBetweenTextAndPageNumberBlocks;

                        tableHeaderName = PdfUtils.GetTable(chapterHeader, PdfGen.HeaderTextBlockWidth);

                        tableHeaderName.WriteSelectedRows(0, -1,
                            headerTableX,
                            document.PageSize.Height - document.TopMargin + 2,
                            writer.DirectContent);

                        #endregion

                        #region Chapter name - show only on 1st page of Chapter
                        if (chapterPage == 0)
                        {
                            chapterName = new Paragraph(nameString, new Font(Goudy, 16, Font.BOLD));
                            space = new Paragraph(6, " ");
                            chapterName.SpacingBefore = 2;
                            chapterName.SpacingAfter = 4;
                            chapterName.Alignment = Element.ALIGN_LEFT;

                            document.Add(space);
                            document.Add(chapterName);
                        }
                        else
                        {
                            space = new Paragraph(" ", new Font(Goudy, 14));
                            document.Add(space);
                        }
                        #endregion

                        HtmlComponentList elements = HtmlUtils.ExtractHTMLElements(PdfUtils.RemoveKindleHtml(chapterPages[chapterPage]));

                        if (elements.JustPlainText)
                        {
                            if (!string.IsNullOrEmpty(chapterPages[chapterPage]))
                            {
                                chapterText = new Paragraph(PdfGen.LeadingNormalText, chapterPages[chapterPage], new Font(Helvetica, 9));
                                chapterText.SpacingAfter = 4;
                                document.Add(chapterText);
                            }
                        }
                        else
                        {
                            chapterRichText = PdfUtils.GetRichTextForPdf(elements, Helvetica, 9, PdfGen.LeadingNormalText, 4, Parameters.SkipImageExceptions);

                            FillDocumentWithRichText(ref document, chapterRichText);

                        }
                        // ebook - body chapters
                        if (ChapterPageBreaksInParagraph[chapterPage]) // if previous chapter page breaks in paragraph then delete initial <p>
                        {   // chapterPage must begin with </p>
                            chapterPages[chapterPage] = chapterPages[chapterPage].Substring(3);
                        }
                        if (ChapterPageBreaksInParagraph[chapterPage+1]) // if this chapter page breaks in paragraph then delete final </p> and add a space
                        {   // chapterPage must end in </p>
                            chapterPages[chapterPage] = chapterPages[chapterPage].Substring(0, chapterPages[chapterPage].Length - 4);
                        }
                        ebookFile.WriteLine(PdfUtils.GetKindleHtml(PdfUtils.GetHTMLSafeCharacters(chapterPages[chapterPage])));
                        // NOTE: may be special cases where page breaks on punctuation (eg hypehated word) - but none found in content 181205 so not handling

                        document.NewPage();
                    }
                    debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                    debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done chapter:" + chaptersBody[cb].ChapterName);
                    debugLogFile.Close();

                    CountChapterPagesGenerated++;
                    if (Parameters.FirstSectionOnly && CountChapterPagesGenerated == 3)
                        break;
                }

                debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done chapters");
                debugLogFile.Close();
                #endregion

                pageHelper.ShowTipsBackgroundsFromThisPageOn = true;

                sections = sections.OrderBy(s => s.DisplayOrder).ToList();

                #region Tips
                int CountTipPagesGenerated = 0;
                for (int s = 0; s < sections.Count; s++)
                {
                    List<BookPage> tipsInThisSection = pages.Where(p =>
                        p.BookSection.Id.HasValue &&
                        p.BookSection.Id.Value == sections[s].Id).ToList();

                    tipsInThisSection = tipsInThisSection.OrderBy(p => p.DisplayOrder).ToList();

                    for (int i = 0; i < tipsInThisSection.Count; i++)
                    {
                        #region UQ
                        updateQueue.Add(
                        new UpdateQueueFirstLastPage
                        {
                            EntityId = tipsInThisSection[i].Id.Value,
                            Type = PdfPageType.Tip,
                            PageFirst = writer.PageNumber,
                            PageLast = writer.PageNumber
                        });
                        #endregion

                        BookPage currentPage = tipsInThisSection[i];
                        string description = currentPage.BookPageDescription;

                        imagesCost = PdfUtils.SetImagesForValue(currentPage.Cost *2, DotsImages); // Dec18: using values 1-5 (not 1-10) 
                        imagesDifficulty = PdfUtils.SetImagesForValue(currentPage.Difficulty *2, DotsImages);
                        //imagesImpact = PdfUtils.SetImagesForImpact(currentPage.LifeExtension40 *2, DotsImages);
                        imagesImpact = PdfUtils.SetImagesForValue(currentPage.LifeExtension40 * 2, DotsImages); // Dec18: not using LE40 range, just 1-5
                        PdfContentByte canvas = writer.DirectContent;

                        // ebook
                        ebookFile.WriteLine(EBook.EbookPageBreak);

                        #region Section Name - in orange header

                        Paragraph section = new Paragraph(
                            currentPage.BookSection.BookSectionName.ToUpper(),
                            new Font(Helvetica, 12, Font.BOLD, PdfGen.White));
                        section.Alignment = Element.ALIGN_CENTER;

                        sectionTableX = (writer.PageNumber % 2 > 0) ?
                            0 : PdfGen.HeaderPageNumberBlockWidth + PdfGen.HeaderSpaceBetweenTextAndPageNumberBlocks;

                        PdfPTable tableSectionName = PdfUtils.GetTable(section, PdfGen.HeaderTextBlockWidth);

                        tableSectionName.WriteSelectedRows(0, -1,
                            sectionTableX,
                            document.PageSize.Height - document.TopMargin + 2,
                            writer.DirectContent);

                        // ebook
                        //ebookFile.WriteLine("<div class=\"pageheader\"><h2 id=\"tip-" + currentPage.Id.ToString() + "\" class=\"pageheader\">" + currentPage.BookSection.BookSectionName.ToUpper() + "</h2></div>");
                        ebookFile.WriteLine("\r\n<h2 id=\"tip-" + currentPage.Id.ToString() + "\" class=\"pageheader\">" + currentPage.BookSection.BookSectionName.ToUpper() + "</h2>");

                        #endregion

                        #region Separators

                        space = new Paragraph(" ", new Font(Goudy, 16));
                        document.Add(space);

                        #endregion

                        #region Cost + difficulty + impact score bars

                        PdfPTable tableCost = new PdfPTable(6);

                        tableCost.SetTotalWidth(new float[] {
                            PdfGen.ScoreBarCostWidth - 75, 15, 15, 15, 15, 15 });

                        tableCost.AddCell(
                            PdfUtils.CreateTextCell(PdfGen.Cost, Element.ALIGN_RIGHT));
                        for (int ic = 0; ic < imagesCost.Count(); ic++)
                        {
                            tableCost.AddCell(PdfUtils.CreateImageCell(imagesCost[ic], Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
                        }

                        PdfPTable tableDifficulty = new PdfPTable(6);

                        tableDifficulty.SetTotalWidth(new float[] {
                            PdfGen.ScoreBarEaseWidth - 75, 15, 15, 15, 15, 15 });
                        
                        tableDifficulty.AddCell(
                            PdfUtils.CreateTextCell(PdfGen.Difficulty, Element.ALIGN_RIGHT));
                        for (int id = 0; id < imagesDifficulty.Count(); id++)
                        {
                            tableDifficulty.AddCell(PdfUtils.CreateImageCell(imagesDifficulty[id], Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
                        }

                        PdfPTable tableImpact = new PdfPTable(6);
                        tableImpact.SetTotalWidth(new float[] {
                            PdfGen.ScoreBarImpactWidth - 75, 15, 15, 15, 15, 15 });
                        tableImpact.AddCell(
                            PdfUtils.CreateTextCell(PdfGen.Impact, Element.ALIGN_RIGHT));
                        for (int ii = 0; ii < imagesImpact.Count(); ii++)
                        {
                            tableImpact.AddCell(PdfUtils.CreateImageCell(imagesImpact[ii], Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
                        }

                        PdfPTable scoreBars = new PdfPTable(3);
                        scoreBars.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                        scoreBars.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        scoreBars.SetTotalWidth(new float[]
                            {
                                PdfGen.ScoreBarCostWidth,
                                PdfGen.ScoreBarEaseWidth,
                                PdfGen.ScoreBarImpactWidth
                            });
                        
                        PdfPCell costCell = new PdfPCell(tableCost);
                        PdfPCell difficultyCell = new PdfPCell(tableDifficulty);
                        PdfPCell impactCell = new PdfPCell(tableImpact);

                        PdfUtils.SetCellProperties(costCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE);
                        //costCell.Border = Rectangle.BOX;//for alignment tests
                        costCell.AddElement(tableCost);

                        PdfUtils.SetCellProperties(difficultyCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE);
                        //difficultyCell.Border = Rectangle.BOX;//for alignment tests
                        difficultyCell.AddElement(tableDifficulty);

                        PdfUtils.SetCellProperties(impactCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE);
                        //impactCell.Border = Rectangle.BOX; //for alignment tests
                        impactCell.AddElement(tableImpact);

                        scoreBars.AddCell(costCell);
                        scoreBars.AddCell(difficultyCell);
                        scoreBars.AddCell(impactCell);

                        float scoresX = (writer.PageNumber % 2 > 0) ?
                            document.LeftMargin - PdfGen.Adjustment02 :
                            document.LeftMargin - PdfGen.Adjustment02 - (0.16f * PdfGen.Inch);

                        scoreBars.WriteSelectedRows(0, -1,
                            scoresX,
                            document.PageSize.Height -
                            PdfGen.HeaderHeight + 5,
                            writer.DirectContent);

                        // ebook
                        // changed to key score labels in image (I couldn't get Kindle to center align)
                        // ebookFile.WriteLine("\r\n<table class=\"tip-score\"><tr><td class=\"tip-score\">" + PdfGen.Cost + "</td><td class=\"tip-score\">" + PdfGen.Difficulty + "</td><td class=\"tip-score\">" + PdfGen.Impact + "</td></tr>");
                        // ebookFile.WriteLine("\r\n<tr><td><img src=\"keyscore" + (currentPage.Cost).ToString() + ".jpg\"></td><td><img src=\"keyscore" + currentPage.Difficulty.ToString() + ".jpg\"></td><td><img src=\"keyscore" + currentPage.LifeExtension40.ToString() + ".jpg\"></td></tr></table>");
                        ebookFile.WriteLine("\r\n<table class=\"tip-score\"><tr><td><img src=\"keyscores-cost-" + (currentPage.Cost).ToString() + ".png\"></td><td><img src=\"keyscores-ease-" + currentPage.Difficulty.ToString() + ".png\"></td><td><img src=\"keyscores-impact-" + currentPage.LifeExtension40.ToString() + ".png\"></td></tr></table>");

                        debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                        debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done tip score bar (" + currentPage.BookPageName + ")");
                        debugLogFile.Close();
                        #endregion

                        #region Text - Title + description

                        pageTitle = new Paragraph(currentPage.BookPageName, new Font(Goudy, 16, Font.BOLD));
                        pageTitle.SpacingAfter = 4;
                        pageTitle.Alignment = Element.ALIGN_LEFT;
                        document.Add(pageTitle);

                        // ebook
                        ebookFile.WriteLine("\r\n<h1>" + currentPage.BookPageName + "</h1>");

                        HtmlComponentList elements = HtmlUtils.ExtractHTMLElements(description);

                        if (elements.JustPlainText)
                        {
                            if (!string.IsNullOrEmpty(description))
                            {
                                tipText = new Paragraph(PdfGen.LeadingNormalText, description, new Font(Helvetica, 9));
                                tipText.SpacingAfter = 4;
                                document.Add(tipText);
                            }
                        }
                        else
                        {
                            tipRichText = PdfUtils.GetRichTextForPdf(elements, Helvetica, 9, PdfGen.LeadingNormalText, 4, Parameters.SkipImageExceptions);

                            FillDocumentWithRichText(ref document, tipRichText);
                        }

                        // ebook
                        ebookFile.WriteLine(PdfUtils.GetHTMLSafeCharacters(description));
                        debugLogFile = new System.IO.StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                        debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done tip Title + description (" + currentPage.BookPageName + ")");
                        debugLogFile.Close();

                        #endregion

                        #region Tip Main Image -- Fixed position

                        string imageFileName = currentPage.ImageFilename;
                        if (string.IsNullOrEmpty(imageFileName))
                        {
                            if (writer.PageNumber % 2 > 0)
                                imageFileName = PdfGen.PlaceholderImageOdd;
                            else
                                imageFileName = PdfGen.PlaceholderImageEven;
                        }

                        string imageFullPath = Path.Combine(
                            Parameters.PdfImagesPath, imageFileName); //FixedImage
                        try
                        {
                            image = pdf.Image.GetInstance(imageFullPath);
                        }
                        catch (Exception ex)
                        {
                            result.IsSuccessfulWithWarnings = true;
                            result.Warnings.Add(ex.Message);

                            image = pdf.Image.GetInstance(Path.Combine(Parameters.PdfImagesPath, PdfGen.PlaceholderImage));
                        }

                        image.Alignment = (writer.PageNumber % 2 > 0) ?
                            Element.ALIGN_LEFT :
                            Element.ALIGN_RIGHT;

                        //to keep aspect ratio: set H = W
                        //image.ScaleToFit(ImageWidth, ImageHeight);

                        image.ScalePercent(24f);
                        //Uncomment to add Border to image, to test alignment:
                        //image.Border = Rectangle.BOX;
                        //image.BorderColor = BaseColor.BLACK;
                        //image.BorderWidth = 1f;

                        if (writer.PageNumber % 2 > 0)
                        {
                            image.SetAbsolutePosition(0,
                                document.PageSize.Height - (4.34f * PdfGen.Inch) - PdfGen.ImageHeight);
                        }
                        else
                        {
                            image.SetAbsolutePosition(
                                document.PageSize.Width - PdfGen.ImageWidth,
                                document.PageSize.Height - (4.34f * PdfGen.Inch) - PdfGen.ImageHeight);
                        }

                        image.Alt = "No image Found";

                        document.Add(image);

                        debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                        debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done tip image (" + currentPage.BookPageName + ")");
                        debugLogFile.Close();

                        #endregion

                        #region Tips

                        string tipsStr = currentPage.Tips;

                        // ebook
                        ebookFile.WriteLine("\r\n<table><tr><td class=\"tip-actions\"><h3 class=\"tip-no\"><img class=\"tip-no\" src=\"tip-kindle.jpg\">Tip #" + (CountTipPagesGenerated + 1)  + "</h3>");

                        if (!string.IsNullOrEmpty(tipsStr))
                        {
                            string[] tipsList = tipsStr.Split(new string[] { "\r\n" }, StringSplitOptions.None); // tips will be entered with normal line breaks in Add/Edit Page

                            // ebook
                            ebookFile.WriteLine("\r\n<ul class=\"tip-actions\">");

                            PdfPCell tipsCell = new PdfPCell();
                            List list = new List(false, 10);
                            string blueArrowPath = Path.Combine(Parameters.AdditionalImagesPath, "tips-bullet-points.png");
                            pdf.Image bulletImage = pdf.Image.GetInstance(blueArrowPath);
                            bulletImage.ScaleAbsolute(8, 8);
                            bulletImage.ScaleToFitHeight = false;
                            list.ListSymbol = new Chunk(bulletImage, 0, 0);

                            foreach (string tip in tipsList)
                            {
                                list.Add(new ListItem(15, tip, new Font(Goudy, 10, Font.BOLD)));
                                // ebook
                                ebookFile.WriteLine("<li>" + PdfUtils.GetHTMLSafeCharacters(tip) + "</li>");
                            }
                            tipsCell.AddElement(list);
                            tipsCell.Border = Rectangle.NO_BORDER;

                            // ebook
                            ebookFile.WriteLine("</ul></td>");

                            float tipsY = document.PageSize.Height - (4.34f * PdfGen.Inch);
                            float tipsX = (writer.PageNumber % 2 > 0) ?
                                document.PageSize.Width - PdfGen.TipsWidth :
                                PdfGen.ChevronImageWidth + 0.03f * PdfGen.Inch;

                            PdfPTable tableTips = new PdfPTable(1);
                            tableTips.TotalWidth = PdfGen.TipsWidth -
                                PdfGen.ChevronImageWidth -
                                (0.03f * PdfGen.Inch);

                            tableTips.AddCell(tipsCell);

                            tableTips.WriteSelectedRows(0, -1, tipsX, tipsY, canvas);

                            Phrase tipNumber = new Phrase("Tip #" + (CountTipPagesGenerated + 1),
                                new Font(Goudy, 18, Font.BOLD, PdfGen.White));
                            //0.19 Inch is the size of the current font for Tip #12 text. (Goudy 18 => aprox 0.19 inches) 
                            //If we change font size there, we'll change it here too.
                            float tipNumberX = (writer.PageNumber % 2 > 0) ?
                                document.PageSize.Width - PdfGen.MarginRight + PdfGen.Adjustment02 - (0.06f * PdfGen.Inch)  /* - (0.19f*Inch)*/:
                                PdfGen.MarginRight - PdfGen.Adjustment02 + (0.06f * PdfGen.Inch) /*+ (0.19f * Inch)*/;
                            float tipNumberY = document.PageSize.Height -
                                (4.34f * PdfGen.Inch) -
                                (PdfGen.TipsHeight / 2);
                            float rotation = (writer.PageNumber % 2 > 0) ? 90 : -90;

                            ColumnText.ShowTextAligned(canvas,
                                Element.ALIGN_CENTER, tipNumber,
                                tipNumberX, tipNumberY, rotation);

                        }
                        debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                        debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done tip tips (" + currentPage.BookPageName + ")");
                        debugLogFile.Close();
                        #endregion

                        #region Resources

                        string resourcesStr = currentPage.Resources;

                        HtmlComponentList rElements = HtmlUtils.ExtractHTMLElementsPAndH(resourcesStr);

                        float resourcesX = (writer.PageNumber % 2 > 0) ?
                                PdfGen.ImageWidth + PdfGen.TipImageSpace :
                                document.RightMargin; // Left/Right margin values don't alternates as page changes with document.SetMarginMirroring(true) - assume only used when generation file
                        float resourcesY = document.BottomMargin + PdfGen.ResearchPapersBoxHeight + PdfGen.TipImageSpace +
                            PdfGen.ImageHeight - PdfGen.TipsHeight;

                        PdfPTable tableResources = new PdfPTable(1);
                        tableResources.TotalWidth = document.PageSize.Width - (document.RightMargin + PdfGen.ImageWidth + PdfGen.TipImageSpace); // use outside margin (in AppConstants this is MarginRight) = 0.375 as image is positioned over inside margin

                        if (rElements.JustPlainText)
                        {
                            if (!string.IsNullOrEmpty(resourcesStr))
                            {
                                PdfPCell cell = new PdfPCell();
                                cell.Border = Rectangle.NO_BORDER; // see also GetRichTextForPdfResourcesSection below for border when HTML
                                Paragraph resourcesParagraph = new Paragraph(PdfGen.LeadingNormalText, resourcesStr, new Font(Helvetica, 9));
                                resourcesParagraph.SpacingAfter = 4;
                                cell.AddElement(resourcesParagraph);

                                tableResources.AddCell(cell);

                                tableResources.WriteSelectedRows(0, -1,
                                    resourcesX, resourcesY, canvas);
                            }
                        }
                        else
                        {
                            PdfPCell cellResources = PdfUtils.GetRichTextForPdfResourcesSection(
                                rElements.Elements, Goudy, Helvetica, 9, PdfGen.LeadingNormalText, 4);

                            tableResources.AddCell(cellResources);

                            tableResources.WriteSelectedRows(0, -1,
                                resourcesX, resourcesY, canvas);
                        }
                        debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                        debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done tip resources (" + currentPage.BookPageName + ")");
                        debugLogFile.Close();

                        // ebook
                        ebookFile.WriteLine("<td width=\"30%\" class=\"tip-image\"><img src=\"" + imageFileName + "\" class=\"tip-image\"/></td></tr></table>"); // images are in same folder as HTML for KDP zip file
                        ebookFile.WriteLine(PdfUtils.GetKindleHtml(PdfUtils.GetHTMLSafeCharacters(currentPage.Resources)));
                        #endregion

                        #region Research Papers
                        PdfPTable tableResearchPapers = new PdfPTable(1);
                        PdfPCell researchPapersCell = new PdfPCell();
                        // Paragraph researchPapers = new Paragraph(10, currentPage.ResearchPapers, new Font(Helvetica, 8));
                        // researchPapers.SpacingAfter = 3;
                        //sets the x to be the biggest margin between left/right
                        float researchPapersX = (writer.PageNumber % 2 > 0) ?
                            PdfGen.MarginLeft : PdfGen.MarginRight;
                        float researchPapersY = PdfGen.MarginBottomForGraphics +
                            PdfGen.ResearchPapersBoxHeight + 0.06f * PdfGen.Inch;

                        HtmlComponentList researchpaperElements = HtmlUtils.ExtractHTMLElements(currentPage.ResearchPapers);

                        if (researchpaperElements.JustPlainText)
                        {
                            if (!string.IsNullOrEmpty(currentPage.ResearchPapers))
                            {
                                researchpaperText = new Paragraph(PdfGen.LeadingResearchPapers, currentPage.ResearchPapers, new Font(Helvetica, 8));
                                researchpaperText.SpacingAfter = 3;
                                researchPapersCell.AddElement(researchpaperText);
                            }
                        }
                        else
                        {
                            researchpaperRichText = PdfUtils.GetRichTextForPdf(researchpaperElements, Helvetica, 8, PdfGen.LeadingResearchPapers, 3, Parameters.SkipImageExceptions);
                            for (int p = 0; p < researchpaperRichText.Count; p++)
                            {
                                if (researchpaperRichText[p].GetType() == typeof(Paragraph))
                                {
                                    Paragraph paragraph = (Paragraph)researchpaperRichText[p];
                                    researchPapersCell.AddElement(paragraph);
                                }
                                if (researchpaperRichText[p].GetType() == typeof(pdf.Image))
                                {
                                    pdf.Image pdfImage = (pdf.Image)researchpaperRichText[p];
                                    researchPapersCell.AddElement(pdfImage);
                                }

                                if (researchpaperRichText[p].GetType() == typeof(PdfPTable))
                                {
                                    PdfPTable table = (PdfPTable)researchpaperRichText[p];
                                    researchPapersCell.AddElement(table);
                                }
                            }
                        }

                        // researchPapersCell.AddElement(researchPapers);
                        researchPapersCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        researchPapersCell.Border = Rectangle.NO_BORDER;

                        tableResearchPapers.TotalWidth = document.PageSize.Width
                            - document.LeftMargin - document.RightMargin;
                        tableResearchPapers.AddCell(researchPapersCell);

                        tableResearchPapers.WriteSelectedRows(0, -1,
                            researchPapersX, researchPapersY,
                            canvas);

                        // ebook
                        ebookFile.WriteLine("\r\n<div class=\"researchpapers\">" + PdfUtils.GetKindleHtml(PdfUtils.GetHTMLSafeCharacters(currentPage.ResearchPapers)) + "</div>");
                        debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                        debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done tip Research Papers (" + currentPage.BookPageName + ")");
                        debugLogFile.Close();
                        #endregion

                        document.NewPage();

                        debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                        debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done tip: " + currentPage.BookPageName);
                        debugLogFile.Close();

                        CountTipPagesGenerated++;
                        if (Parameters.FirstSectionOnly && CountTipPagesGenerated == 3)
                            break;
                    }
                    if (Parameters.FirstSectionOnly && CountTipPagesGenerated == 3)
                    {
                        break;
                    }
                }
                debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done tips");
                debugLogFile.Close();
                #endregion

                #region Back Chapters

                // if (!Parameters.FirstSectionOnly)
                // {
                    pageHelper.ShowHeaderAndPageNumberFromThisPageOn = false;
                    pageHelper.ShowTipsBackgroundsFromThisPageOn = false;

                    chaptersBack = chaptersBack.OrderBy(c => c.DisplayOrder).ToList();
                    for (int cb = 0; cb < chaptersBack.Count; cb++)
                    {
                        List<string> chapterPages = PdfUtils.GetAllChapterPages(chaptersBack[cb]);

                        updateQueue.Add(
                            new UpdateQueueFirstLastPage
                            {
                                EntityId = chaptersBack[cb].Id.Value,
                                Type = PdfPageType.BackChapter,
                                PageFirst = writer.PageNumber,
                                PageLast = writer.PageNumber + chapterPages.Count - 1
                            });

                        for (int chapterPage = 0; chapterPage < chapterPages.Count; chapterPage++)
                        {
                            // ebook
                            ebookFile.WriteLine(EBook.EbookPageBreak);
                            ebookFile.WriteLine("\r\n<div id=\"chapter-" + chaptersBack[cb].Id +"\"></div>");

                        HtmlComponentList elements = HtmlUtils.ExtractHTMLElements(PdfUtils.RemoveKindleHtml(chapterPages[chapterPage]));

                            if (writer.PageNumber % 2 > 0)
                                size = new Rectangle(document.LeftMargin,
                                PdfGen.ResearchPapersBoxHeight + document.BottomMargin,
                                document.PageSize.Width - document.RightMargin,
                                document.PageSize.Height - document.TopMargin -
                                ((float)chaptersBack[cb].MarginTop * PdfGen.Inch));
                            else
                                size = new Rectangle(document.RightMargin,
                                PdfGen.ResearchPapersBoxHeight + document.BottomMargin,
                                document.PageSize.Width - document.LeftMargin,
                                document.PageSize.Height - document.TopMargin -
                                ((float)chaptersBack[cb].MarginTop * PdfGen.Inch));


                            ct.SetSimpleColumn(size);

                            if (elements.JustPlainText)
                            {
                                if (!string.IsNullOrEmpty(chapterPages[chapterPage]))
                                {
                                    chapterText = new Paragraph(PdfGen.LeadingNormalText, chapterPages[chapterPage], new Font(Helvetica, 9));
                                    chapterText.SpacingAfter = 4;
                                    ct.AddElement(chapterText);
                                }
                            }
                            else
                            {
                                chapterRichText = PdfUtils.GetRichTextForPdf(elements, Helvetica, 9, PdfGen.LeadingNormalText, 4, Parameters.SkipImageExceptions);

                                FillColumnWithRichText(ref ct, chapterRichText);
                            }
                            ct.Go();

                            document.NewPage();

                            // ebook - back chapters
                            ebookFile.WriteLine(PdfUtils.GetKindleHtml(PdfUtils.GetHTMLSafeCharacters(chapterPages[chapterPage])));
                        }

                    }
                // }

                debugLogFile = new StreamWriter(Path.Combine(Parameters.PdfsRoot, "debug-pdf-gen.txt"), append: true);
                debugLogFile.WriteLine(DateTime.Now.ToString("yyyyMMdd:HHmm") + ": Done back matter");
                debugLogFile.Close();
                #endregion

                document.Close();

                // ebook
                ebookFile.WriteLine(EBook.EbookHtmlBackCover);
                ebookFile.WriteLine(EBook.EbookHtmlFooter);
                ebookFile.Close();

                result.NameOfFileResulted = fileName;
                result.IsSuccessful = true;
                result.Message = "Generation Successful.";
                result.UpdateQueueFirstLastPages = updateQueue;
                return result;
            }
            catch (ImageNotFoundException ie)
            {
                document.Close();
                ebookFile.Close();
                throw ie;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Message = ex.Message;
                // close files if an error
                document.Close();
                ebookFile.Close();
                return result;
            }
            finally
            {
                ebookFile.Close();
                document.Close();
            }

        }
        
        public PdfGenerationResultViewModel GenerateOnePage(BookPage page, bool oddPage)
        {
            PdfGenerationResultViewModel result = new PdfGenerationResultViewModel();

            LoadFonts(Parameters.FontsPath);
            PdfUtils.ImagesPath = Parameters.PdfImagesPath;

            string fileName = $"PageID-[{page.Id}]-Manual_{ DateTime.Now.Date.ToString("yyyyMMdd")}-{DateTime.Now.ToString("HHmm")}.pdf";

            string fullOutputPath = Path.Combine(Parameters.PdfsRoot, fileName);
            pdf.Image image;
            Rectangle pageSize = new Rectangle(PdfGen.PageWidth, PdfGen.PageHeight);

            float marginLeft = oddPage ? PdfGen.MarginLeft : PdfGen.MarginRight;
            float marginRight = oddPage? PdfGen.MarginRight : PdfGen.MarginLeft;

            Document document = new Document(pageSize, marginLeft, marginRight,
                PdfGen.MarginTop, PdfGen.MarginBottomForText);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(fullOutputPath, FileMode.Create));

            pdf.Image[] imagesCost = new pdf.Image[5];
            pdf.Image[] imagesDifficulty = new pdf.Image[5];
            pdf.Image[] imagesImpact = new pdf.Image[5];

            DotsImages = PdfUtils.GetDotsImages(Parameters.AdditionalImagesPath);
            imagesCost = PdfUtils.SetImagesForValue(page.Cost, DotsImages);
            imagesDifficulty = PdfUtils.SetImagesForValue(page.Difficulty, DotsImages);
            imagesImpact = PdfUtils.SetImagesForImpact(page.LifeExtension40, DotsImages);
            
            Paragraph emptyParagraph = new Paragraph(" ");
            float sectionTableX;
            Paragraph pageTitle;

            PdfPageHelper pageHelper = new PdfPageHelper(Parameters.FooterText, Parameters.AdditionalImagesPath);
            pageHelper.WritingFont = Helvetica;
            pageHelper.ShowHeaderAndPageNumberFromThisPageOn = true;
            pageHelper.ShowTipsBackgroundsFromThisPageOn = true;
            writer.PageEvent = pageHelper;
            pageHelper.ForceEvenPage = (oddPage == false);

            document.Open();
            PdfContentByte canvas = writer.DirectContent;
            Paragraph section = new Paragraph(
                page.BookSection.BookSectionName.ToUpper(),
                new Font(Helvetica, 12, Font.BOLD, PdfGen.White));
            section.Alignment = Element.ALIGN_CENTER;

            sectionTableX = oddPage ? 0 : PdfGen.HeaderPageNumberBlockWidth + PdfGen.HeaderSpaceBetweenTextAndPageNumberBlocks;

            PdfPTable tableSectionName = PdfUtils.GetTable(section, PdfGen.HeaderTextBlockWidth);

            tableSectionName.WriteSelectedRows(0, -1,
                sectionTableX,
                document.PageSize.Height - document.TopMargin + 2,
                writer.DirectContent);

            var space = new Paragraph(" ", new Font(Goudy, 16));
            document.Add(space);

            PdfPTable tableCost = new PdfPTable(6);

            tableCost.SetTotalWidth(new float[] { PdfGen.ScoreBarCostWidth - 75, 15, 15, 15, 15, 15 });

            tableCost.AddCell(
                PdfUtils.CreateTextCell(PdfGen.Cost, Element.ALIGN_RIGHT));
            for (int ic = 0; ic < imagesCost.Count(); ic++)
            {
                tableCost.AddCell(PdfUtils.CreateImageCell(imagesCost[ic], Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
            }

            PdfPTable tableDifficulty = new PdfPTable(6);
            tableDifficulty.SetTotalWidth(new float[] { PdfGen.ScoreBarEaseWidth - 75, 15, 15, 15, 15, 15 });

            tableDifficulty.AddCell(
                PdfUtils.CreateTextCell(PdfGen.Difficulty, Element.ALIGN_RIGHT));
            for (int id = 0; id < imagesDifficulty.Count(); id++)
            {
                tableDifficulty.AddCell(PdfUtils.CreateImageCell(imagesDifficulty[id], Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
            }

            PdfPTable tableImpact = new PdfPTable(6);
            tableImpact.SetTotalWidth(new float[] { PdfGen.ScoreBarImpactWidth - 75, 15, 15, 15, 15, 15 });

            tableImpact.AddCell(
                PdfUtils.CreateTextCell(PdfGen.Impact, Element.ALIGN_RIGHT));
            for (int ii = 0; ii < imagesImpact.Count(); ii++)
            {
                tableImpact.AddCell(PdfUtils.CreateImageCell(imagesImpact[ii], Element.ALIGN_CENTER, Element.ALIGN_BOTTOM));
            }

            PdfPTable scoreBars = new PdfPTable(3);
            scoreBars.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
            scoreBars.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            scoreBars.SetTotalWidth(new float[] 
                {
                    PdfGen.ScoreBarCostWidth,
                    PdfGen.ScoreBarEaseWidth,
                    PdfGen.ScoreBarImpactWidth
                });

            PdfPCell costCell = new PdfPCell(tableCost);
            PdfPCell difficultyCell = new PdfPCell(tableDifficulty);
            PdfPCell impactCell = new PdfPCell(tableImpact);

            PdfUtils.SetCellProperties(costCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE);
            costCell.AddElement(tableCost);

            PdfUtils.SetCellProperties(difficultyCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE);
            difficultyCell.AddElement(tableDifficulty);

            PdfUtils.SetCellProperties(impactCell, Element.ALIGN_LEFT, Element.ALIGN_MIDDLE);
            impactCell.AddElement(tableImpact);

            scoreBars.AddCell(costCell);
            scoreBars.AddCell(difficultyCell);
            scoreBars.AddCell(impactCell);

            scoreBars.WriteSelectedRows(0, -1,
                document.LeftMargin,
                document.PageSize.Height -
                PdfGen.HeaderHeight + 5,
                writer.DirectContent);

            pageTitle = new Paragraph(page.BookPageName, new Font(Goudy, 16, Font.BOLD));
            pageTitle.SpacingAfter = 4;
            pageTitle.Alignment = Element.ALIGN_LEFT;
            document.Add(pageTitle);

            HtmlComponentList elements = HtmlUtils.ExtractHTMLElements(page.BookPageDescription);
            if (elements.JustPlainText)
            {
                if (!string.IsNullOrEmpty(page.BookPageDescription))
                {
                    var tipText = new Paragraph(PdfGen.LeadingNormalText, page.BookPageDescription, new Font(Helvetica, 9));
                    tipText.SpacingAfter = 4;
                    document.Add(tipText);
                }
            }
            else
            {
                var tipRichText = PdfUtils.GetRichTextForPdf(elements, Helvetica, 9, PdfGen.LeadingNormalText, 4, Parameters.SkipImageExceptions);
                for (int p = 0; p < tipRichText.Count; p++)
                {
                    if (tipRichText[p].GetType() == typeof(Paragraph))
                    {
                        Paragraph paragraph = (Paragraph)tipRichText[p];
                        document.Add(paragraph);
                    }
                    else if (tipRichText[p].GetType() == typeof(pdf.Image))
                    {
                        pdf.Image pdfImage = (pdf.Image)tipRichText[p];
                        document.Add(pdfImage);
                    }
                    else if (tipRichText[p].GetType() == typeof(PdfPTable))
                    {
                        PdfPTable table = (PdfPTable)tipRichText[p];
                        document.Add(table);
                    }
                }
            }

            string imageFullPath = Path.Combine(Parameters.PdfImagesPath, page.ImageFilename);
            try
            {
                image = pdf.Image.GetInstance(imageFullPath);
            }
            catch (Exception imgEx)
            {
                image = pdf.Image.GetInstance(Path.Combine(Parameters.PdfImagesPath, PdfGen.PlaceholderImage));
            }

            image.Alignment = oddPage ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
            
            image.ScalePercent(24f);
            
            if (oddPage)
            {
                image.SetAbsolutePosition(0,
                    document.PageSize.Height - (4.34f * PdfGen.Inch) - PdfGen.ImageHeight);
            }
            else
            {
                image.SetAbsolutePosition(
                    document.PageSize.Width - PdfGen.ImageWidth,
                    document.PageSize.Height - (4.34f * PdfGen.Inch) - PdfGen.ImageHeight);
            }

            image.Alt = "No image Found";

            document.Add(image);
            
            string tipsStr = page.Tips;
            
            if (!string.IsNullOrEmpty(tipsStr))
            {
                string[] tipsList = tipsStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                
                PdfPCell tipsCell = new PdfPCell();
                List list = new List(false, 10);
                string blueArrowPath = Path.Combine(Parameters.AdditionalImagesPath, "tips-bullet-points.png");
                pdf.Image bulletImage = pdf.Image.GetInstance(blueArrowPath);
                bulletImage.ScaleAbsolute(8, 8);
                bulletImage.ScaleToFitHeight = false;
                list.ListSymbol = new Chunk(bulletImage, 0, 0);

                foreach (string tip in tipsList)
                {
                    list.Add(new ListItem(15, tip, new Font(Goudy, 10, Font.BOLD)));
                }
                tipsCell.AddElement(list);
                tipsCell.Border = Rectangle.NO_BORDER;
                
                float tipsY = document.PageSize.Height - (4.34f * PdfGen.Inch);
                float tipsX = oddPage ?
                    document.PageSize.Width - PdfGen.TipsWidth :
                    PdfGen.ChevronImageWidth + 0.03f * PdfGen.Inch;

                PdfPTable tableTips = new PdfPTable(1);
                tableTips.TotalWidth = PdfGen.TipsWidth -
                    PdfGen.ChevronImageWidth -
                    (0.03f * PdfGen.Inch);

                tableTips.AddCell(tipsCell);

                tableTips.WriteSelectedRows(0, -1, tipsX, tipsY, canvas);

                Phrase tipNumber = new Phrase("Tip #X",
                    new Font(Goudy, 18, Font.BOLD, PdfGen.White));

                float tipNumberX = oddPage ?
                    document.PageSize.Width - PdfGen.MarginRight :
                    PdfGen.MarginRight;
                float tipNumberY = document.PageSize.Height -
                    (4.34f * PdfGen.Inch) - (PdfGen.TipsHeight / 2);
                float rotation = oddPage ? 90 : -90;

                ColumnText.ShowTextAligned(canvas,
                    Element.ALIGN_CENTER,
                    tipNumber,
                    tipNumberX, tipNumberY, rotation);
                
                string resourcesStr = page.Resources;

                HtmlComponentList rElements = HtmlUtils.ExtractHTMLElementsPAndH(resourcesStr);

                float resourcesX = oddPage ?
                        PdfGen.ImageWidth + PdfGen.TipImageSpace :
                        document.RightMargin;
                float resourcesY = document.BottomMargin + PdfGen.ResearchPapersBoxHeight + PdfGen.TipImageSpace +
                    PdfGen.ImageHeight - PdfGen.TipsHeight;

                PdfPTable tableResources = new PdfPTable(1);
                tableResources.TotalWidth = document.PageSize.Width - (document.RightMargin + PdfGen.ImageWidth + PdfGen.TipImageSpace);

                if (rElements.JustPlainText)
                {
                    if (!string.IsNullOrEmpty(resourcesStr))
                    {
                        PdfPCell cell = new PdfPCell();
                        cell.Border = Rectangle.NO_BORDER; 
                        Paragraph resourcesParagraph = new Paragraph(PdfGen.LeadingNormalText, resourcesStr, new Font(Helvetica, 9));
                        resourcesParagraph.SpacingAfter = 4;
                        cell.AddElement(resourcesParagraph);

                        tableResources.AddCell(cell);

                        tableResources.WriteSelectedRows(0, -1,
                            resourcesX, resourcesY, canvas);
                    }
                }
                else
                {
                    PdfPCell cellResources = PdfUtils.GetRichTextForPdfResourcesSection(
                        rElements.Elements, Goudy, Helvetica, 9, PdfGen.LeadingNormalText, 4);
                    tableResources.AddCell(cellResources);

                    tableResources.WriteSelectedRows(0, -1,
                        resourcesX, resourcesY, canvas);
                }
            }
            PdfPTable tableResearchPapers = new PdfPTable(1);
            PdfPCell researchPapersCell = new PdfPCell();
            
            float researchPapersX = oddPage ? PdfGen.MarginLeft : PdfGen.MarginRight;
            float researchPapersY = document.BottomMargin +
                PdfGen.ResearchPapersBoxHeight + 0.06f * PdfGen.Inch;

            HtmlComponentList researchpaperElements = HtmlUtils.ExtractHTMLElements(page.ResearchPapers);

            if (researchpaperElements.JustPlainText)
            {
                if (!string.IsNullOrEmpty(page.ResearchPapers))
                {
                    var researchpaperText = new Paragraph(PdfGen.LeadingResearchPapers, page.ResearchPapers, new Font(Helvetica, 8));
                    researchpaperText.SpacingAfter = 3;
                    researchPapersCell.AddElement(researchpaperText);
                }
            }
            else
            {
                var researchpaperRichText = PdfUtils.GetRichTextForPdf(researchpaperElements, Helvetica, 8, PdfGen.LeadingResearchPapers, 3, Parameters.SkipImageExceptions);
                for (int p = 0; p < researchpaperRichText.Count; p++)
                {
                    if (researchpaperRichText[p].GetType() == typeof(Paragraph))
                    {
                        Paragraph paragraph = (Paragraph)researchpaperRichText[p];
                        researchPapersCell.AddElement(paragraph);
                    }
                    if (researchpaperRichText[p].GetType() == typeof(pdf.Image))
                    {
                        pdf.Image pdfImage = (pdf.Image)researchpaperRichText[p];
                        researchPapersCell.AddElement(pdfImage);
                    }

                    if (researchpaperRichText[p].GetType() == typeof(PdfPTable))
                    {
                        PdfPTable table = (PdfPTable)researchpaperRichText[p];
                        researchPapersCell.AddElement(table);
                    }
                }
            }
            researchPapersCell.HorizontalAlignment = Element.ALIGN_LEFT;
            researchPapersCell.Border = Rectangle.NO_BORDER;

            tableResearchPapers.TotalWidth = document.PageSize.Width
                - document.LeftMargin - document.RightMargin;
            tableResearchPapers.AddCell(researchPapersCell);

            tableResearchPapers.WriteSelectedRows(0, -1,
                researchPapersX, researchPapersY,
                canvas);

            document.Close();

            result.NameOfFileResulted = fileName;
            result.IsSuccessful = true;
            result.Message = "Generation Successful.";
            result.IsSinglePageMode = true;

            return result;
        }

        public PdfGenerationResultViewModel GenerateOneChapter(Chapter chapter)
        {
            PdfGenerationResultViewModel result = new PdfGenerationResultViewModel();

            LoadFonts(Parameters.FontsPath);
            PdfUtils.ImagesPath = Parameters.PdfImagesPath;

            string fileName = $"ChapterID-[{chapter.Id}]-Manual_{ DateTime.Now.Date.ToString("yyyyMMdd")}-{DateTime.Now.ToString("HHmm")}.pdf";
            string fullOutputPath = Path.Combine(Parameters.PdfsRoot, fileName);
            
            Rectangle pageSize = new Rectangle(PdfGen.PageWidth, PdfGen.PageHeight);
            
            Document document = new Document(pageSize, PdfGen.MarginLeft, PdfGen.MarginRight, PdfGen.MarginTop, PdfGen.MarginBottomForText);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(fullOutputPath, FileMode.Create));
            List<string> chapterPages = PdfUtils.GetAllChapterPages(chapter);
            string nameString = chapter.ChapterName;

            PdfPageHelper pageHelper = new PdfPageHelper(Parameters.FooterText, Parameters.AdditionalImagesPath);
            pageHelper.WritingFont = Helvetica;
            pageHelper.ShowTipsBackgroundsFromThisPageOn = false;
            writer.PageEvent = pageHelper;
            document.Open();
            Rectangle size; ColumnText ct = new ColumnText(writer.DirectContent);
            try
            {
                //chapter is type front or back
                if (chapter.TypeID == 10 || chapter.TypeID == 30)
                {
                    pageHelper.ShowHeaderAndPageNumberFromThisPageOn = false;
                    for (int chapterPage = 0; chapterPage < chapterPages.Count; chapterPage++)
                    {
                        HtmlComponentList elements = HtmlUtils.ExtractHTMLElements(chapterPages[chapterPage]);
                        if (writer.PageNumber % 2 > 0)
                            size = new Rectangle(
                            document.LeftMargin,
                            PdfGen.ResearchPapersBoxHeight + document.BottomMargin,
                            document.PageSize.Width - document.RightMargin,
                            document.PageSize.Height - document.TopMargin -
                            ((float)chapter.MarginTop * PdfGen.Inch));
                        else
                            size = new Rectangle(
                            document.RightMargin,
                            PdfGen.ResearchPapersBoxHeight + document.BottomMargin,
                            document.PageSize.Width - document.LeftMargin,
                            document.PageSize.Height - document.TopMargin -
                            ((float)chapter.MarginTop * PdfGen.Inch));

                        ct.SetSimpleColumn(size);

                        if (elements.JustPlainText)
                        {
                            if (!string.IsNullOrEmpty(chapterPages[chapterPage]))
                            {
                                var chapterText = new Paragraph(PdfGen.LeadingNormalText, chapterPages[chapterPage], new Font(Helvetica, 9));
                                chapterText.SpacingAfter = 4;
                                ct.AddElement(chapterText);
                            }
                        }
                        else
                        {
                            var chapterRichText = PdfUtils.GetRichTextForPdf(elements, Helvetica, 9, PdfGen.LeadingNormalText, 4, Parameters.SkipImageExceptions);
                            FillColumnWithRichText(ref ct, chapterRichText);
                        }
                        ct.Go();

                        document.NewPage();
                    }
                }
                else
                {
                    pageHelper.ShowHeaderAndPageNumberFromThisPageOn = true;

                    for (int chapterPage = 0; chapterPage < chapterPages.Count; chapterPage++)
                    {
                        var chapterHeader = new Paragraph(nameString.ToUpper(),
                            new Font(Helvetica, 12, Font.BOLD, PdfGen.White));
                        chapterHeader.Alignment = Element.ALIGN_CENTER;

                        var headerTableX = (writer.PageNumber % 2 > 0) ? 0 :
                        PdfGen.HeaderPageNumberBlockWidth + PdfGen.HeaderSpaceBetweenTextAndPageNumberBlocks;

                        var tableHeaderName = PdfUtils.GetTable(chapterHeader, PdfGen.HeaderTextBlockWidth);

                        tableHeaderName.WriteSelectedRows(0, -1,
                            headerTableX,
                            document.PageSize.Height - document.TopMargin + 2,
                            writer.DirectContent);

                        if (chapterPage == 0)
                        {
                            var chapterName = new Paragraph(nameString, new Font(Goudy, 16, Font.BOLD));
                            var space = new Paragraph(6, " ");
                            chapterName.SpacingBefore = 2;
                            chapterName.SpacingAfter = 4;
                            chapterName.Alignment = Element.ALIGN_LEFT;

                            document.Add(space);
                            document.Add(chapterName);
                        }
                        else
                        {
                            var space = new Paragraph(" ", new Font(Goudy, 14));
                            document.Add(space);
                        }

                        HtmlComponentList elements = HtmlUtils.ExtractHTMLElements(chapterPages[chapterPage]);

                        if (elements.JustPlainText)
                        {
                            if (!string.IsNullOrEmpty(chapterPages[chapterPage]))
                            {
                                var chapterText = new Paragraph(PdfGen.LeadingNormalText, chapterPages[chapterPage], new Font(Helvetica, 9));
                                chapterText.SpacingAfter = 4;
                                document.Add(chapterText);
                            }
                        }
                        else
                        {
                            var chapterRichText = PdfUtils.GetRichTextForPdf(elements, Helvetica, 9, PdfGen.LeadingNormalText, 4, Parameters.SkipImageExceptions);
                            FillDocumentWithRichText(ref document, chapterRichText);
                        }

                        document.NewPage();
                    }
                }

                result.NameOfFileResulted = fileName;
                result.IsSuccessful = true;
                result.Message = "Generation Successful.";
                result.IsSingleChapterMode = true;
                return result;
            }
            catch (ImageNotFoundException ie)
            {
                throw ie;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.Message = ex.Message;
                document.Close();
                return result;
            }
            finally
            {
                document.Close();
            }
        }

        private void FillColumnWithRichText(ref ColumnText ct, List<IAccessibleElement> richText)
        {
            for (int p = 0; p < richText.Count; p++)
            {
                if (richText[p].GetType() == typeof(Paragraph))
                {
                    Paragraph paragraph = (Paragraph)richText[p];
                    ct.AddElement(paragraph);
                }
                else if (richText[p].GetType() == typeof(pdf.ImgRaw))
                {
                    var pdfImage = (pdf.ImgRaw)richText[p];
                    pdfImage.ScalePercent(24);
                    ct.AddElement(pdfImage);
                }
                else if (richText[p].GetType() == typeof(pdf.Jpeg))
                {
                    var jpgImage = (pdf.Jpeg)richText[p];
                    jpgImage.ScalePercent(24);
                    ct.AddElement(jpgImage);
                }

                else if (richText[p].GetType() == typeof(PdfPTable))
                {
                    PdfPTable table = (PdfPTable)richText[p];
                    ct.AddElement(table);
                }
            }
        }

        private void FillDocumentWithRichText(ref Document document, List<IAccessibleElement> richText)
        {
            for (int p = 0; p < richText.Count; p++)
            {
                var typeOf = richText[p].GetType();

                if (richText[p].GetType() == typeof(Paragraph))
                {
                    Paragraph paragraph = (Paragraph)richText[p];
                    document.Add(paragraph);
                }

                else if (richText[p].GetType() == typeof(ImgRaw))
                {
                    var pdfImage = (pdf.ImgRaw)richText[p];
                    pdfImage.ScalePercent(24);
                    document.Add(pdfImage);
                }

                else if (richText[p].GetType() == typeof(pdf.Jpeg))
                {
                    var jpgImage = (pdf.Jpeg)richText[p];
                    jpgImage.ScalePercent(24);
                    document.Add(jpgImage);
                }

                else if (richText[p].GetType() == typeof(PdfPTable))
                {
                    PdfPTable table = (PdfPTable)richText[p];
                    document.Add(table);
                }
            }
        }
    }
}