using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Elixir.Models.Utils;
using Elixir.Models;
using pdf = iTextSharp.text;
using iTextSharp.text.pdf.interfaces;
using static Elixir.Models.Utils.AppConstants;
using Elixir.Areas.AdminManual.Models;

namespace Elixir.Helpers
{ 
    public static class PdfUtils
    {
        public static string ImagesPath;

        public static pdf.Image[] SetImagesForValue(int? value, Dictionary<string, pdf.Image> points)
        {
            //first, fill all 5 with empty points, then add 
            //each full/half point, one by one
            pdf.Image[] images = new pdf.Image[5];
            for (int i = 0; i < 5; i++)
                images[i] = points["Empty"];
            if (!value.HasValue)
                return images;

            double score = (double)value / 2;
            if (score < 1 && score > 0)
            {
                images[0] = points["Half"];
                return images;
            }
            if (score >= 1)
            {
                images[0] = points["Full"];
                if (score == 1.5)
                {
                    images[1] = points["Half"];
                    return images;
                }
            }
            if (score >= 2)
            {
                images[1] = points["Full"];
                if (score == 2.5)
                {
                    images[2] = points["Half"];
                    return images;
                }
            }
            if (score >= 3)
            {
                images[2] = points["Full"];
                if (score == 3.5)
                {
                    images[3] = points["Half"];
                    return images;
                }
            }
            if (score >= 4)
            {
                images[3] = points["Full"];
                if (score == 4.5)
                {
                    images[4] = points["Half"];
                    return images;
                }
            }
            if (score == 5)
            {
                images[4] = points["Full"];
                return images;
            }

            return images;
        }

        public static pdf.Image[] SetImagesForImpact(int? value, Dictionary<string, pdf.Image> points)
        {
            pdf.Image[] images = new pdf.Image[5];
            for (int i = 0; i < 5; i++)
                images[i] = points["Empty"];
            if (!value.HasValue)
                return images;
            if(value >= 1 && value <= 2)
            {
                images[0] = points["Half"]; return images;
            }
            if(value >= 3 && value <= 4)
            {
                images[0] = points["Full"]; return images;
            }
            if (value >= 5 && value <= 8)
            {
                images[0] = points["Full"];
                images[1] = points["Half"];
                return images;
            }
            if (value >= 9 && value <= 16)
            {
                images[0] = points["Full"];
                images[1] = points["Full"];
                return images;
            }
            if (value >= 17 && value <= 32)
            {
                images[0] = points["Full"];
                images[1] = points["Full"];
                images[2] = points["Half"];
                return images;
            }
            if (value >= 33 && value <= 64)
            {
                images[0] = points["Full"];
                images[1] = points["Full"];
                images[2] = points["Full"];
                return images;
            }
            if (value >= 65 && value <= 128)
            {
                images[0] = points["Full"];
                images[1] = points["Full"];
                images[2] = points["Full"];
                images[3] = points["Half"];
                return images;
            }
            if (value >= 129 && value <= 256)
            {
                images[0] = points["Full"];
                images[1] = points["Full"];
                images[2] = points["Full"];
                images[3] = points["Full"];
                return images;
            }
            if (value >= 257 && value <= 512)
            {
                images[0] = points["Full"];
                images[1] = points["Full"];
                images[2] = points["Full"];
                images[3] = points["Full"];
                images[4] = points["Half"];
                return images;
            }
            if(value >= 513)
            {
                images[0] = points["Full"];
                images[1] = points["Full"];
                images[2] = points["Full"];
                images[3] = points["Full"];
                images[4] = points["Full"];
                return images;
            }
            
            return images;
        }

        public static Dictionary<string, pdf.Image> GetDotsImages(string dotsPath)
        {
            Dictionary<string, pdf.Image> dotsImages = new Dictionary<string, pdf.Image>();
            pdf.Image imageDotFull = pdf.Image.GetInstance(Path.Combine(dotsPath, "dot-full.png"));
            pdf.Image imageDotHalf = pdf.Image.GetInstance(Path.Combine(dotsPath, "dot-half.png"));
            pdf.Image imageDotEmpty = pdf.Image.GetInstance(Path.Combine(dotsPath, "dot-empty.png"));
            imageDotFull.ScaleToFit(10, 10);
            imageDotHalf.ScaleToFit(10, 10);
            imageDotEmpty.ScaleToFit(10, 10);

            dotsImages.Add("Full", imageDotFull);
            dotsImages.Add("Half", imageDotHalf);
            dotsImages.Add("Empty", imageDotEmpty);

            return dotsImages;
        }

        public static PdfPCell CreateTextCell(string text, int textAlignment, int verticalAlignment = 0)
        {
            Paragraph p = new Paragraph(text,
               new Font(Font.FontFamily.TIMES_ROMAN, 10));
            p.Alignment = textAlignment;
            PdfPCell cell = new PdfPCell();
            cell.AddElement(p);
            if(verticalAlignment != 0)
                cell.VerticalAlignment = verticalAlignment;
            cell.Border = Rectangle.NO_BORDER;

            return cell;
        }

        public static PdfPCell CreateTextCellWithFont(string text, BaseFont fontName, int fontSize, int textAlignment, int verticalAlignment = 0)
        {
            Paragraph p = new Paragraph(11, text, new Font(fontName, fontSize));
            p.Alignment = textAlignment;
            PdfPCell cell = new PdfPCell();
            cell.AddElement(p);
            if (verticalAlignment != 0)
                cell.VerticalAlignment = verticalAlignment;
            cell.Border = Rectangle.NO_BORDER;

            return cell;
        }

        public static PdfPCell CreateImageCell(pdf.Image image, int horizontalAlign, int verticalAlign)
        {
            image.Border = Rectangle.NO_BORDER;
            PdfPCell cell = new PdfPCell(image);
            cell.HorizontalAlignment = horizontalAlign;
            cell.VerticalAlignment = verticalAlign;

            cell.Border = Rectangle.NO_BORDER;
            return cell;
        }
        
        public static void SetCellProperties(PdfPCell cell, int horizontalAlign, int verticalAlign)
        {
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = horizontalAlign;
            cell.VerticalAlignment = verticalAlign;
        }
        
        public static float ToPoints(this float pixels)
        {
            return pixels / 110 * 72;
        }
        
        public static PdfPTable GetTable(Paragraph textInTable, float width)
        {
            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = width;

            PdfPCell cell = new PdfPCell(textInTable);

            cell.Border = Rectangle.NO_BORDER;
            cell.PaddingTop = -1;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(cell);

            return table; 
        }

        public static List<IAccessibleElement> GetRichTextForPdf(HtmlComponentList elements, BaseFont font, int size, int leading, int spacingAfter, bool skipImagesExceptions = false)
        {
            List<IAccessibleElement> list = new List<IAccessibleElement>();

            for (int el = 0; el < elements.Elements.Count; el++)
            {
                if (elements.Elements[el].IsAdded) continue;

                HtmlTag current = elements.Elements[el];
                
                string type = current.GetType().Name;
                switch (type)
                {
                    #region H2
                    case "H2":
                    {
                        // embedded tags within h2 not supported - i.e. no paragraphs, strong, BR, etc - just plain text
                        H2 h2 = (H2)current;
                        Paragraph h2Par = new Paragraph(leading, h2.Text , new Font(font, size + 2, Font.BOLD)); // h2 = 2 point larger than normal text  and bold
                        h2Par.SpacingAfter = PdfGen.SpacingAfterH2;
                        list.Add(h2Par);
                        break;
                    }
                    #endregion

                    #region P
                    case "P":
                    {
                        P p = (P)current;
                        Paragraph par = new Paragraph(leading, p.Text, new Font(font, size));
                        par.SpacingAfter = spacingAfter;
                        SetParagraphAlignment(par, p.Align);

                        if (p.HasStrongChunks || p.HasItalicChunks)
                        {
                            Paragraph richParagraph = new Paragraph();
                            richParagraph.SetLeading(leading, 0);
                            richParagraph.SpacingAfter = spacingAfter;
                            int strongIndexStart = p.Text.IndexOf("<strong>");
                            int strongIndexEnd = p.Text.IndexOf("</strong>");
                            int emIndexStart = p.Text.IndexOf("<em>");
                            int emIndexEnd = p.Text.IndexOf("</em>");
                            while((strongIndexStart >= 0 && strongIndexEnd >= 0) ||
                                (emIndexStart >= 0 && emIndexEnd >= 0))
                            {
                                var seIndex = GetFirstNestedTag(strongIndexStart, emIndexStart);
                                string strongOrEmText = null;
                                string normalText = p.Text.Substring(0, seIndex.Item2);
                                    if (seIndex.Item1 == 's')
                                    {
                                        strongOrEmText = p.Text.Substring(
                                        strongIndexStart + "<strong>".Length,
                                        strongIndexEnd - strongIndexStart - "<strong>".Length);
                                    }
                                    else
                                    {
                                        strongOrEmText = p.Text.Substring(
                                        emIndexStart + "<em>".Length,
                                        emIndexEnd - emIndexStart - "<em>".Length);
                                    }

                                int footnoteStartIndex = normalText.IndexOf("<footnote>");
                                int supStartIndex = normalText.IndexOf("<sup>");
                                if (footnoteStartIndex >= 0 || supStartIndex >= 0)
                                {
                                    richParagraph.AddTextWithSuperscripts(normalText, font, size);
                                }
                                else
                                {
                                    richParagraph.Add(new Chunk(normalText, new Font(font, size)));
                                }

                                if (seIndex.Item1 == 's')
                                {
                                    richParagraph.Add(new Chunk(strongOrEmText, 
                                        new Font(font, size, Font.BOLD)));
                                }
                                else if (seIndex.Item1 == 'e')
                                {
                                    richParagraph.Add(new Chunk(strongOrEmText,
                                    new Font(font, size, Font.ITALIC)));
                                }
                                    
                                int nextSearch = seIndex.Item1 == 's' ? 
                                    strongIndexEnd + "</strong>".Length :
                                    emIndexEnd + "</em>".Length;
                                
                                p.Text = p.Text.Substring(nextSearch);

                                strongIndexStart = p.Text.IndexOf("<strong>");
                                strongIndexEnd = p.Text.IndexOf("</strong>");
                                emIndexStart = p.Text.IndexOf("<em>");
                                emIndexEnd = p.Text.IndexOf("</em>");
                            }

                            //if there's any remaining text not bold/italic:
                            if (p.Text.Length > 0)
                            {
                                richParagraph.Add(new Chunk(p.Text, new Font(font, size)));
                            }

                            SetParagraphAlignment(richParagraph, p.Align);
                                
                            list.Add(richParagraph);
                        }
                        else
                        {
                            if (p.HasBreaks)
                            {
                                Paragraph pLine;
                                string[] lines = p.Text.Split(new string[] { "<br>" }, StringSplitOptions.None);
                                for (int l = 0; l < lines.Length; l++)
                                {
                                    string line = lines[l].Trim();
                                    pLine = new Paragraph(leading, line, new Font(font, size));
                                    if (p.Align == Align.Center)
                                        pLine.Alignment = Element.ALIGN_CENTER;
                                    else
                                        pLine.Alignment = Element.ALIGN_LEFT;
                                        if (l == (lines.Length - 1)) // don't put SpacingAfter after the line break(s) - but do after end of whole paragraph
                                        pLine.SpacingAfter = spacingAfter;
                                    else
                                        pLine.SpacingAfter = 0;
                                    list.Add(pLine);
                                }

                            }
                            else
                            {
                                Paragraph richParagraph = new Paragraph(leading, "", new Font(font, size));
                                richParagraph.SpacingAfter = spacingAfter;
                                SetParagraphAlignment(richParagraph, p.Align);

                                int footnoteStartIndex = p.Text.IndexOf("<footnote>");
                                int supStartIndex = p.Text.IndexOf("<sup>");
                                if (footnoteStartIndex >= 0 || supStartIndex >= 0)
                                {
                                    richParagraph.AddTextWithSuperscripts(p.Text, font, size);
                                }
                                else
                                {
                                    richParagraph.Add(new Chunk(p.Text, new Font(font, size)));
                                }
                                list.Add(richParagraph);
                            }
                            
                        } 
                        break;
                    }
                    #endregion

                    #region Strong
                    case "Strong":
                    {
                        Strong s = (Strong)current;
                        list.Add(new Paragraph(leading, new Chunk(s.Text, new Font(font, size, Font.BOLD))));
                        break;
                    }
                    #endregion

                    #region Image
                    case "Image":
                    {
                        Image img = (Image)current;
                        if (img.Align == Align.Left)
                        {
                            HtmlTag nextText = elements.Elements[el + 1];
                            P pNextToImage = (P)nextText;
                            elements.Elements[el + 1].IsAdded = true;
                            PdfPTable tbImagePlusText = new PdfPTable(2)
                            {
                                TotalWidth = PdfGen.SafeZoneWidth,
                                LockedWidth = true,
                                HorizontalAlignment = Element.ALIGN_LEFT
                            };
                            tbImagePlusText.SpacingBefore = 2;
                            try
                            {
                                var pdfImage = pdf.Image.GetInstance(Path.Combine(ImagesPath, img.Src));
                                pdfImage.ScalePercent(24f);

                                PdfPCell imgCell = CreateImageCell(pdfImage,
                                    Element.ALIGN_LEFT, Element.ALIGN_TOP);
                                PdfPCell textCell = CreateTextCellWithFont(pNextToImage.Text,
                                    font, 9, Element.ALIGN_JUSTIFIED, Element.ALIGN_TOP);
                                textCell.UseAscender = true;
                                textCell.UseDescender = true;
                                //calculate table relative widths for image/text

                                tbImagePlusText.HorizontalAlignment = Element.ALIGN_LEFT;

                                float imageCellWidth = pdfImage.ScaledWidth + 6; //added a little space

                                tbImagePlusText.SetTotalWidth(new float[] {
                                imageCellWidth, PdfGen.SafeZoneWidth - imageCellWidth });

                                tbImagePlusText.AddCell(imgCell);
                                tbImagePlusText.AddCell(textCell);

                                list.Add(tbImagePlusText);
                                current.IsAdded = true;
                            }
                            catch (Exception ex)
                            {
                                if (!skipImagesExceptions)
                                    throw new ImageNotFoundException(ex.Message);
                                else continue;
                            }
                        }
                        
                        //TODO: code to support Image Float Right - if needed - take the previous paragraph
                        else if(img.Align == Align.Center)
                        {
                            try
                            {
                                var pdfImage = pdf.Image.GetInstance(Path.Combine(ImagesPath, img.Src));
                                pdfImage.ScalePercent(24f);
                                pdfImage.Alignment = Element.ALIGN_CENTER;
                                list.Add(pdfImage);
                                current.IsAdded = true;
                            }
                            catch(Exception ex)
                            {
                                if (!skipImagesExceptions)
                                    throw new ImageNotFoundException(ex.Message);
                                else continue;
                            }
                        }
                        
                        break;
                    }
                        #endregion
                } 
            }
            return list;
        }

        private static Tuple<char, int> GetFirstNestedTag(int strongIndexStart, int emIndexStart)
        {
            if (strongIndexStart >= 0 && emIndexStart == -1)
                return new Tuple<char, int>('s', strongIndexStart);

            if (emIndexStart >= 0 && strongIndexStart == -1)
                return new Tuple<char, int>('e', emIndexStart);

            if (strongIndexStart >= 0 && emIndexStart >= 0)
            {
                if (strongIndexStart > emIndexStart) return new Tuple<char,int>('e', emIndexStart);
                if (strongIndexStart < emIndexStart) return new Tuple<char, int>('s', strongIndexStart);
            }
            throw new Exception("GetFirstNestedTag: Some invalid format in your indexes/HTML");
        }

        public static PdfPCell GetRichTextForPdfResourcesSection(List<HtmlTag> elements, BaseFont headingsFont, BaseFont paragraphFont, int fontSize, int leading, int spacingAfter)
        {
            PdfPCell cell = new PdfPCell();
            cell.Border = Rectangle.NO_BORDER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;

            for (int e = 0; e < elements.Count; e++)
            {
                HtmlTag current = elements[e];
                string type = current.GetType().Name;
                switch (type)
                {
                    case "H3":
                        {
                            Paragraph h3 = new Paragraph(((H3)current).Text,
                                new Font(headingsFont, 10, Font.BOLD, AppConstants.PdfGen.Blue));
                            cell.AddElement(h3);
                            break;
                        }
                    case "H4":
                        {
                            Paragraph h4 = new Paragraph(((H4)current).Text,
                            new Font(headingsFont, 9, Font.BOLD, AppConstants.PdfGen.LightBlue));
                            cell.AddElement(h4);
                            break;
                        }
                    case "P":
                        {
                            P paragraph = (P)current;

                            if (paragraph.HasBreaks)
                            {
                                Paragraph pLine;
                                string[] lines = paragraph.Text.Split(new string[] { "<br>" }, StringSplitOptions.None);
                                for (int l = 0; l < lines.Length; l++)
                                {
                                    string line = lines[l].Trim();
                                    pLine = new Paragraph(leading, line, new Font(paragraphFont, fontSize));
                                    if (l == (lines.Length - 1)) // don't put SpacingAfter after the line break(s) - but do after end of whole paragraph
                                        pLine.SpacingAfter = spacingAfter;
                                    else
                                        pLine.SpacingAfter = 0;
                                    pLine.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(pLine);
                                }

                            }
                            else
                            {
                                Paragraph pWhole = new Paragraph(leading, paragraph.Text, new Font(paragraphFont, fontSize));
                                pWhole.Alignment = Element.ALIGN_LEFT;
                                pWhole.SpacingAfter = spacingAfter;
                                cell.AddElement(pWhole);
                            }

                            break;
                        }

                    case "B":
                        {
                            Strong boldTag = (Strong)current;

                            Paragraph boldParagraph = new Paragraph(10,
                                boldTag.Text, new Font(paragraphFont, 14, Font.BOLD));
                            boldParagraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(boldParagraph);

                            break;
                        }
                    case "Image":
                        {
                            Image img = (Image)current;
                            pdf.Image image = pdf.Image.GetInstance(img.Src);
                            image.Alignment = Element.ALIGN_CENTER;

                            if (img.Align == Align.Left)
                                image.Alignment = Element.ALIGN_LEFT;
                            if (img.Align == Align.Right)
                                image.Alignment = Element.ALIGN_RIGHT;

                            cell.AddElement(image);
                            break;
                        }

                }
            }

            return cell;
        }

        private static void SetParagraphAlignment(Paragraph par, Align align)
        {
            par.Alignment = Element.ALIGN_JUSTIFIED;
            if (align == Align.Right)
                par.Alignment = Element.ALIGN_RIGHT;
            if (align == Align.Center)
                par.Alignment = Element.ALIGN_CENTER;
        }

        #region Table of Contents
        public static List<TableOfContentItem> GetTableOfContentsItems(
            List<Models.Chapter> chaptersFront, 
            List<Models.Chapter> chaptersBody, 
            List<BookSection> sections, 
            List<BookPage> pages)
        {

            List<TableOfContentItem> Map = new List<TableOfContentItem>();
            //I do this just to fill the 0 index, so pages can start from 1:
            Map.Add(new TableOfContentItem(0, PdfPageType.None, null, 0));
            int page = 1;

            chaptersFront = chaptersFront.OrderBy(c => c.DisplayOrder).ToList();
            chaptersBody = chaptersBody.OrderBy(c => c.DisplayOrder).ToList();
            sections = sections.OrderBy(s => s.DisplayOrder).ToList();

            for (int cf = 0; cf < chaptersFront.Count; cf++)
            {
                int pagesInChapter = GetAllChapterPages(chaptersFront[cf]).Count;
                Map.Add(new TableOfContentItem(page, PdfPageType.FrontChapter, chaptersFront[cf].ChapterName, chaptersFront[cf].Id));
                page += pagesInChapter;
            }

            //TOC - how many pages: total lines to write / fixed number of TOC lines per page (30?)
            int tableOfContentLines = chaptersFront.Count + chaptersBody.Count +sections.Count + pages.Count;
            int tableOfContentPages = tableOfContentLines / AppConstants.PdfGen.TableOfContentsLinesPerPage;
            if (tableOfContentLines % AppConstants.PdfGen.TableOfContentsLinesPerPage != 0)
                tableOfContentPages++;

            while (tableOfContentPages > 0)
            {
                Map.Add(new TableOfContentItem(page, PdfPageType.TableOfContents, "Table of contents", null));
                page++;
                tableOfContentPages--;
            }
            
            for(int cb = 0; cb < chaptersBody.Count; cb++)
            {
                int pagesInChapter = GetAllChapterPages(chaptersBody[cb]).Count;
                Map.Add(new TableOfContentItem(page, PdfPageType.BodyChapter, chaptersBody[cb].ChapterName, chaptersBody[cb].Id));

                page += pagesInChapter;
            }
            
            for (int s = 0; s < sections.Count; s++)
            {
                //Sections will not have a PDF page.
                BookSection currentSection = sections[s];
                Map.Add(new TableOfContentItem(0, PdfPageType.Section, currentSection.BookSectionName, currentSection.Id));

                List<BookPage> tipsInCurrentSection = pages.Where(p => 
                    p.BookSection.Id.HasValue &&
                    p.BookSection.Id == currentSection.Id).OrderBy(p=> 
                    p.DisplayOrder).ToList();
                
                for(int t = 0; t < tipsInCurrentSection.Count; t++)
                {
                    Map.Add(new TableOfContentItem(page, PdfPageType.Tip, tipsInCurrentSection[t].BookPageName, tipsInCurrentSection[t].Id));
                    page++;
                }
            }
            
            return Map;
        }
        #endregion

        public static List<string> GetAllChapterPages(Models.Chapter chapter)
        {
            List<string> chapterPages = new List<string>();
            chapterPages.Add(chapter.Text);

            if (!string.IsNullOrEmpty(chapter.ContentPage2))
                chapterPages.Add(chapter.ContentPage2);
            else
                return chapterPages;

            if (!string.IsNullOrEmpty(chapter.ContentPage3))
                chapterPages.Add(chapter.ContentPage3);
            else
                return chapterPages;

            if (!string.IsNullOrEmpty(chapter.ContentPage4))
                chapterPages.Add(chapter.ContentPage4);
            else
                return chapterPages;

            if (!string.IsNullOrEmpty(chapter.ContentPage5))
                chapterPages.Add(chapter.ContentPage5);
            else
                return chapterPages;
            
            if (!string.IsNullOrEmpty(chapter.ContentPage6))
                chapterPages.Add(chapter.ContentPage6);
            else
                return chapterPages;

            if (!string.IsNullOrEmpty(chapter.ContentPage7))
                chapterPages.Add(chapter.ContentPage7);
            else
                return chapterPages;

            if (!string.IsNullOrEmpty(chapter.ContentPage8))
                chapterPages.Add(chapter.ContentPage8);
            else
                return chapterPages;

            if (!string.IsNullOrEmpty(chapter.ContentPage9))
                chapterPages.Add(chapter.ContentPage9);
            else
                return chapterPages;

            if (!string.IsNullOrEmpty(chapter.ContentPage10))
                chapterPages.Add(chapter.ContentPage10);
            
            return chapterPages;
        }

        public static string GetHTMLSafeCharacters(string myString)
        {
            if (myString == null)
                return ("");

            string HTMLSafeString = myString.Replace('\u2013', '-').Replace('\u2014', '-').Replace('\u2015', '-').Replace('\u2017', '_').Replace('\u2018', '\'').Replace('\u2019', '\'').Replace('\u201a', ',').Replace('\u201b', '\''); // Smart quotes (Word etc)
            HTMLSafeString = HTMLSafeString.Replace('\u201c', '\"').Replace('\u201d', '\"').Replace('\u201e', '\"').Replace("\u2026", "...").Replace('\u2032', '\'').Replace('\u2033', '\"'); // Smart quotes (Word etc) - cont'd
            HTMLSafeString = HTMLSafeString.Replace("–", "-"); // long dash
            HTMLSafeString = HTMLSafeString.Replace("©", "&copy;").Replace("®", "&reg;").Replace("™", "&trade;"); // copyright, registered, trademark symbol
            HTMLSafeString = HTMLSafeString.Replace("°", "&deg;").Replace("£", "&pound;").Replace("µ", "&micro;"); // degree symbol, pound sign
            HTMLSafeString = HTMLSafeString.Replace("∼", "&sim;").Replace("±", "&plusmn;"); // MATHS: approx (tilde), plus or minus

            return HTMLSafeString;
        } // ENDOF GetHTMLSafeCharacters

        private static string UseKindleImages(string myString) // need lower res images on Kindle than for print - I will create "-kindle" versions, eg "originalfilename-kindle.png"
        {
            if (myString == null)
                return ("");

            // find all img references - for each, find filename and append with -kindle
            int startChar = 0, imgFilenameStart = 0, imgFilenameEnd = 0, imgFilenameDot = 0, LookAheadChars = 0;
            string imgFilename = "HELLO", imgFilenameNew = "HELLO", remainingString = myString;
            while (remainingString.IndexOf("<img src=") >0)
            {
                imgFilenameStart = remainingString.IndexOf("<img src=") + 10;
                imgFilenameEnd = imgFilenameStart + remainingString.Substring(imgFilenameStart).IndexOf("\"");
                if (remainingString.Substring(imgFilenameEnd).Length < 30)
                    LookAheadChars = myString.Substring(imgFilenameEnd).Length;
                else
                    LookAheadChars = 30; // 30 characters gives plenty of leeway for extra space characters - should only need 20
                if (remainingString.Substring(imgFilenameEnd, LookAheadChars).IndexOf("class=\"imgfloatleft\"") > 0) // only use kindle images if floating left - centre aligned images fill full width screen
                {   
                    imgFilename = remainingString.Substring(imgFilenameStart, imgFilenameEnd - imgFilenameStart);
                    imgFilenameDot = imgFilename.LastIndexOf(".");
                    imgFilenameNew = imgFilename.Substring(0, imgFilenameDot) + "-kindle" + imgFilename.Substring(imgFilenameDot);
                    myString = myString.Substring(0, startChar + imgFilenameStart) + imgFilenameNew + remainingString.Substring(imgFilenameEnd);
                }
                startChar = startChar + imgFilenameStart + 10; // safe point past current img tag
                remainingString = myString.Substring(startChar);
            }
            return myString;
        } // ENDOF UseKindleImages
          // NOTE not called for tip images - they are resized in CSS table

        private static string AddAnchorToGoLinks(string myString) // e.g. plain text https://go22.uk/abcdef -> <a href="https://go22.uk/abcdef">https://go22.uk/abcdef</a>
        {
            if (myString == null)
                return ("");

            // manual links (i.e. non-golinks)
            myString = myString.Replace("https://liveforever.club", "<a href=\"https://liveforever.club\">https://liveforever.club</a>");

            // find all golinks - for each, find shortcode, surround with anchor tags
            int startChar = 0, goLinkStart = 0, goLinkEnd = 0;
            int shortCodeStart = -1, shortCodeLength = -1;
            string shortCodeString = "HELLO", remainingString = myString, goLinkAnchor = "HELLO";
            char[] whiteSpaceChars = { ' ', '<', ')', '\r', '\n' }; // need < to catch <br> and </p>
            while (remainingString.IndexOf("https://go22.uk/") > 0)
            {
                goLinkStart = remainingString.IndexOf("https://go22.uk/");
                goLinkEnd = goLinkStart + remainingString.Substring(goLinkStart).IndexOfAny(whiteSpaceChars);
                if (goLinkEnd == goLinkStart - 1) // i.e. whitespace not found (IndexOfAny == -1) - i.e. end of text
                    goLinkEnd = remainingString.Length;
                shortCodeStart = goLinkStart + 16; // length of "https://go22.uk/"
                shortCodeLength = goLinkEnd - shortCodeStart;
                shortCodeString = remainingString.Substring(shortCodeStart, shortCodeLength);
                goLinkAnchor = "<a href=\"https://go22.uk/" + shortCodeString + "\">https://go22.uk/" + shortCodeString + "</a>";
                myString = myString.Substring(0, startChar + goLinkStart) + goLinkAnchor;
                if(goLinkEnd < remainingString.Length) // i.e. shortcode is NOT end of text
                    myString = myString + remainingString.Substring(goLinkEnd);

                startChar = startChar + goLinkStart + goLinkAnchor.Length - 4; // start next search from </a> so don't have to check if end of string in next line
                remainingString = myString.Substring(startChar);
            }
            return myString;
        } // ENDOF AddAnchorToGoLinks
          // NOTE: similar functionality to UseKindleImages - consider generalising into function if do another

        public static string GetKindleHtml(string myString) // call after GetHTMLSafeCharacters so ampersands not replaced
        {
            if (myString == null)
                return ("");

            string KindleHtml = myString.Replace("float=\"left\"", "class=\"imgfloatleft\""); // inline float not supported - but used by Elixir
            KindleHtml = KindleHtml.Replace("<p> </p>\r\n<p> </p>\r\n", "<p> </p>\r\n"); // reduce number of blank lines in ebook - smaller pages and tends to push onto next page (or to bottom)
            KindleHtml = KindleHtml.Replace("<p> </p>", "<p>&nbsp;</p>"); // browsers and Kindle ignore multiple blank paragraphs
            KindleHtml = UseKindleImages(KindleHtml);
            KindleHtml = AddAnchorToGoLinks(KindleHtml); // only works for go links (no other links used in book)

            return KindleHtml;
        } // ENDOF GetKindleHtml

        public static string RemoveKindleHtml(string myString) // call before PDF layout to remove Kindle-only HTML, e.g. <div class="clearfloat"></div>
        {
            if (myString == null)
                return ("");

            string KindleHtmlFreeText = myString.Replace("<div class=\"clearfloat\"></div>", ""); // not used by Elixir, but needed for Kindle, e.g. rich & famous

            return KindleHtmlFreeText;
        } // ENDOF RemoveKindleHtml

        public static void AddTextWithSuperscripts(this Paragraph p, string text, BaseFont font, int size)
        {
            int footnoteStartIndex = text.IndexOf("<footnote>");
            int supStartIndex = text.IndexOf("<sup>");
            int fnTagLen = "<footnote>".Length;
            int startCut = 0;
            while (footnoteStartIndex >= 0)
            {
                int footnoteEndIndex = text.IndexOf("</footnote>", footnoteStartIndex);
                if (footnoteEndIndex >= 0)
                {
                    p.Add(new Chunk(text.Substring(startCut, footnoteStartIndex - startCut), new Font(font, size)));

                    string footnoteRefText = text.Substring(
                        footnoteStartIndex + fnTagLen,
                        footnoteEndIndex - footnoteStartIndex - fnTagLen);

                    var supChunk = new Chunk(footnoteRefText, new Font(font, size - 2));
                    supChunk.SetTextRise(4);
                    p.Add(supChunk);

                    footnoteStartIndex = text.IndexOf("<footnote>", footnoteEndIndex);
                    startCut = footnoteEndIndex + fnTagLen + 1;
                }
                else throw new Exception("HTML has invalid format.");
            }
            if (footnoteStartIndex == -1 && startCut > 0 && startCut < text.Length - 1)
            {
                p.Add(new Chunk(text.Substring(startCut), new Font(font, size)));
            }

            int supTagLen = "<sup>".Length;
            startCut = 0;

            while (supStartIndex >= 0)
            {
                int supEndIndex = text.IndexOf("</sup>", supStartIndex);
                if (supEndIndex >= 0)
                {
                    p.Add(new Chunk(text.Substring(startCut, supStartIndex - startCut)));

                    string supRefText = text.Substring(
                        supStartIndex + supTagLen,
                        supEndIndex - supStartIndex - supTagLen);

                    var supChunk = new Chunk(supRefText, new Font(font, size - 2));
                    supChunk.SetTextRise(4);
                    p.Add(supChunk);

                    supStartIndex = text.IndexOf("<sup>", supEndIndex);
                    startCut = supEndIndex + supTagLen + 1;
                }
                else throw new Exception("HTML has invalid format.");
            }
            if (supStartIndex == -1 && startCut > 0 && startCut < text.Length - 1)
            {
                p.Add(new Chunk(text.Substring(startCut), new Font(font, size)));
            }
        }
    }
    
}