using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.AdminManual.Models;
using Elixir.Areas.AdminManual.ViewModels;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Helpers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Utils;
using static Elixir.Models.Utils.AppConstants;

namespace Elixir.Areas.AdminManual.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator, Roles.Editor)]
    public class ManageBookController : BaseController
    {
        private readonly IPagesProcessor _pagesProcessor;
        private readonly IChaptersProcessor _chaptersProcessor;
        private readonly ISectionsProcessor _sectionsProcessor;

        public string PdfRootPath { get; set; }

        public string ImagesRootPath { get; set; }

        public string AdditionalImagesPath { get; set; }

        public string FontsPath { get; set; }

        public string PdfImagesPath { get; set; }
        
        public ManageBookController(IPagesProcessor pagesProcessor, IChaptersProcessor chaptersProcessor, ISectionsProcessor sectionsProcessor)
        {
            _pagesProcessor = pagesProcessor;
            _chaptersProcessor = chaptersProcessor;
            _sectionsProcessor = sectionsProcessor;
        }
        
        public ActionResult Index()
        {
            return RedirectToAction("Generate");
        }

        [HttpGet]
        public ActionResult Generate(bool skipImageExceptions = false)
        {
            PdfGenerationParametersViewModel viewModel = new PdfGenerationParametersViewModel();
            viewModel.SkipImageExceptions = skipImageExceptions;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate(PdfGenerationParametersViewModel submittedViewModel)
        {
            PdfGenerationResultViewModel pdfGenResultViewModel = new PdfGenerationResultViewModel();
            PathsInitialSetup();

            try
            {
                List<BookPage> pages = _pagesProcessor.GetAllPages(BookDataType.Included, BookPagesSortOrder.DisplayOrder, SortDirection.Ascending).ToList();
                List<Chapter> chaptersFront = _chaptersProcessor.
                    GetChaptersByType(ChapterType.FrontMatter, ChapterIncluded.IncludedOnly).ToList();
                List<Chapter> chaptersBody = _chaptersProcessor.
                    GetChaptersByType(ChapterType.Body, ChapterIncluded.IncludedOnly).ToList();
                List<Chapter> chaptersBack = _chaptersProcessor.
                    GetChaptersByType(ChapterType.BackMatter, ChapterIncluded.IncludedOnly).ToList();
                List<BookSection> sections = _sectionsProcessor.GetAllSections(
                    BookDataType.Included, BookSectionsSortOrder.DisplayOrder, SortDirection.Ascending).ToList();

                PdfGenerationParameters parameters = new PdfGenerationParameters();
                parameters.FooterText = submittedViewModel.FooterText;
                parameters.FirstSectionOnly = submittedViewModel.FirstSectionOnly;
                parameters.SkipImageExceptions = submittedViewModel.SkipImageExceptions;
                parameters.PdfsRoot = PdfRootPath;
                parameters.ImagesRoot = ImagesRootPath;
                parameters.AdditionalImagesPath = AdditionalImagesPath;
                parameters.FontsPath = FontsPath;
                parameters.PdfImagesPath = PdfImagesPath;

                PdfGenerator factory = new PdfGenerator(parameters);
                pdfGenResultViewModel = factory.GeneratePdfFile(
                    pages, chaptersFront, chaptersBody, chaptersBack, sections);

                #region Update FirstPage and LastPage
                if (pdfGenResultViewModel.UpdateQueueFirstLastPages.Count > 0)
                {
                    foreach (var item in pdfGenResultViewModel.UpdateQueueFirstLastPages)
                    {
                        switch (item.Type)
                        {
                            case PdfPageType.FrontChapter:
                            case PdfPageType.BodyChapter:
                            case PdfPageType.BackChapter:
                                {
                                    _chaptersProcessor.SetChapterFirstPageLastPage(item.EntityId, item.PageFirst, item.PageLast);
                                    break;
                                }
                            case PdfPageType.Tip:
                                {
                                    _pagesProcessor.SetPageFirstPageLastPage(item.EntityId, item.PageFirst, item.PageLast);
                                    break;
                                }
                        }
                    }
                }
                #endregion

                return View("Result", pdfGenResultViewModel);
            }
            catch (ImageNotFoundException ie)
            {
                pdfGenResultViewModel.IsSuccessful = false;
                pdfGenResultViewModel.ShowButtonRetryAndSkipImageErrors = true;
                pdfGenResultViewModel.Message = "Image not found:" + ie.MissingFile;
                
                return View("Result", pdfGenResultViewModel);
            }
            catch (Exception ex)
            {
                pdfGenResultViewModel.IsSuccessful = false;
                pdfGenResultViewModel.Message = "Error while fetching pages from DB: " + ex.Message;
                return View("Result", pdfGenResultViewModel);
            }
        }
        
        public ActionResult GenerateSinglePage(int pageId, bool actAsOddPage, bool skipImageExceptions = false)
        {
            PathsInitialSetup();
            
            var page = _pagesProcessor.GetPageById(pageId);
            var section = _sectionsProcessor.GetSectionById(page.BookSection.Id.Value);
            PdfGenerationResultViewModel onepageResultViewModel = new PdfGenerationResultViewModel();

            PdfGenerationParameters parameters = new PdfGenerationParameters(
                this.PdfRootPath, this.ImagesRootPath, this.AdditionalImagesPath, this.FontsPath, this.PdfImagesPath);
            parameters.SkipImageExceptions = skipImageExceptions;

            PdfGenerator factory = new PdfGenerator(parameters);
            onepageResultViewModel = factory.GenerateOnePage(page, actAsOddPage);

            return View("Result", onepageResultViewModel);
        }

        public ActionResult GenerateSingleChapter(int chapterId, bool skipImageExceptions = false)
        {
            PathsInitialSetup();

            var chapter = _chaptersProcessor.GetChapterById(chapterId);
            
            PdfGenerationResultViewModel result = new PdfGenerationResultViewModel();

            PdfGenerationParameters parameters = new PdfGenerationParameters(
                this.PdfRootPath, this.ImagesRootPath, this.AdditionalImagesPath, this.FontsPath, this.PdfImagesPath);
            parameters.SkipImageExceptions = skipImageExceptions;
            try
            {
                PdfGenerator factory = new PdfGenerator(parameters);
                result = factory.GenerateOneChapter(chapter);
            }
            catch (ImageNotFoundException ie)
            {
                result.IsSuccessful = false;
                result.ShowButtonRetryAndSkipImageErrors = true;
                result.Message = "Image not found:" + ie.MissingFile;
            }
            catch (Exception e)
            {
                result.IsSuccessful = false;
                result.Message = "PDF (Chapter Mode) Error: " + e.Message;
            }
            return View("Result", result);
        }
        
        public ActionResult Open(string fileName)
        {
            string pdfRootPath = Server.MapPath("~/MyPdfs");
            string filePath = Path.Combine(pdfRootPath, fileName);

            byte[] bytes = System.IO.File.ReadAllBytes(filePath);

            return File(bytes, "application/pdf");
        }

        public FileResult Save(string fileName)
        {
            string pdfRootPath = Server.MapPath("~/MyPdfs");
            string filePath = Path.Combine(pdfRootPath, fileName);
            string contentType = "application/pdf";

            return File(filePath, contentType, "book.pdf");
        }

        private void PathsInitialSetup()
        {
            PdfRootPath = Server.MapPath("~/" + PdfGen.FolderToSavePdfs);
            ImagesRootPath = Server.MapPath("~/Images");
            AdditionalImagesPath = Server.MapPath("~/Content/Images");
            FontsPath = Server.MapPath("~/Content/Fonts");
            PdfImagesPath = Server.MapPath("~/App_Data/Images/Manual");
        }
    }
}