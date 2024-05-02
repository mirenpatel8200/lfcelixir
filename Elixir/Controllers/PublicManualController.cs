using Elixir.Areas.AdminManual.Models;
using Elixir.Areas.AdminManual.ViewModels;
using Elixir.Contracts.Interfaces;
using Elixir.Helpers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.ViewModels;
using Elixir.ViewModels.Enums;
using Elixir.Views;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Elixir.Models.Utils.AppConstants;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class PublicManualController : BaseController
    {

        public string PdfRootPath { get; set; }

        public string ImagesRootPath { get; set; }

        public string AdditionalImagesPath { get; set; }

        public string FontsPath { get; set; }

        public string PdfImagesPath { get; set; }

        private readonly IPagesProcessor _pagesProcessor;
        private readonly IChaptersProcessor _chaptersProcessor;
        private readonly ISettingsProcessor _settingsProcessor;

        public PublicManualController(
            IPagesProcessor pagesProcessor,
            IChaptersProcessor chaptersProcessor,
            ISettingsProcessor settingsProcessor)
        {
            _pagesProcessor = pagesProcessor;
            _chaptersProcessor = chaptersProcessor;
            _settingsProcessor = settingsProcessor;
        }

        [HttpGet]
        [Route("manual/2018/{pageNumber}")]
        public ActionResult ViewPage(int pageNumber)
        {
            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);

            var isVisibleNewFlashText = _settingsProcessor.GetSettingsByName("IsVisibleNewFlashText");
            if (isVisibleNewFlashText != null)
            {
                ViewData.AddOrUpdateValue(ViewDataKeys.IsVisibleNewFlashText, isVisibleNewFlashText);
                if (!string.IsNullOrEmpty(isVisibleNewFlashText.PairValue) && Convert.ToBoolean(isVisibleNewFlashText.PairValue))
                {
                    var newFlashText = _settingsProcessor.GetSettingsByName("NewFlashText");
                    if (newFlashText != null)
                        ViewData.AddOrUpdateValue(ViewDataKeys.NewFlashText, newFlashText);
                }
            }

            //if (!Request.IsAuthenticated)
            //{
            //    throw new ContentNotFoundException("Page not found");
            //}
            ManualComponentViewModel model = new ManualComponentViewModel();
            model.MemberMode = Request.IsAuthenticated;

            model.IsAuthenticated = User.Identity.IsAuthenticated;
            model.IsLongevist = User.IsInRole(Roles.Longevist.ToString());
            model.IsSupporter = User.IsInRole(Roles.Supporter.ToString());

            if (pageNumber < 0 || pageNumber > OnlineManualHelper.LastManualPage)
            {
                model.Description = "Page does not exist";
                model.Type = ManualComponentType.OutOfRange;
            }
            if (pageNumber.IsChapterPage())
            {
                model.Type = ManualComponentType.ChapterPage;
                var chapterEntity = _chaptersProcessor.GetChapterByManualPageNumber(pageNumber);

                if (chapterEntity == null)
                {
                    throw new ContentNotFoundException("Chapter does not exist");
                }

                model.Name = GetChapterTitle(chapterEntity);
                model.IsChapterFirstPage = IsChapterFirstPage(chapterEntity, pageNumber);
                model.Description = GetChapterPageContent(chapterEntity, pageNumber);
                if (!string.IsNullOrEmpty(model.Description))
                    model.Description = ReplaceImageSources(model.Description);
                model.PageNumber = pageNumber;
                model.Title = model.Name + " - page " + model.PageNumber;
                ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription,
                    "Page " + model.PageNumber + " of the Live Forever Manual – " + model.Name);
                ViewBag.Title = model.Name + " - page " + model.PageNumber;
                if (chapterEntity.TypeID == (int)ChapterType.FrontMatter)
                    model.Type = ManualComponentType.ChapterFront;
                if (chapterEntity.TypeID == (int)ChapterType.BackMatter)
                    model.Type = ManualComponentType.ChapterBack;

                return View(model);
            }
            if (pageNumber.IsTipPage())
            {
                model.Type = ManualComponentType.Tip;

                var tipEntity = _pagesProcessor.GetPageByManualPageNumber(pageNumber);

                model.Name = tipEntity.BookPageName;
                model.Description = tipEntity.BookPageDescription;
                model.TipNumber = pageNumber - 67;
                model.PageNumber = pageNumber;
                model.SectionName = tipEntity.BookSection.BookSectionName;
                model.Cost = tipEntity.Cost;
                model.Ease = tipEntity.Difficulty;
                model.Impact = tipEntity.LifeExtension40;
                model.Tips = tipEntity.Tips;
                model.Resources = tipEntity.Resources;
                model.TipImage = tipEntity.ImageFilename;
                model.Research = tipEntity.ResearchPapers;
                model.Title = model.Name;
                ViewBag.Title = model.Name;
                ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription,
                    "Page " + model.PageNumber + " of the Live Forever Manual – Life Extension Tip #" + model.TipNumber + " – " + model.Name);

                return View(model);
            }
            if (pageNumber == 0)
            {
                model.Type = ManualComponentType.CoverFront;
                model.PageNumber = pageNumber;
                model.Description = "Front cover coming soon";
                model.Title = "Front Cover";
                ViewBag.Title = "Front Cover";
                model.ImageSrc = "/Images/liveforevermanual/cover-6x9-front.jpg";
                ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, "Front Cover of the Live Forever Manual");
            }
            if (pageNumber == OnlineManualHelper.LastManualPage)
            {
                model.Type = ManualComponentType.CoverBack;
                model.PageNumber = pageNumber;
                model.Description = "Back cover coming soon";
                model.Title = "Back Cover";
                model.ImageSrc = "/Images/liveforevermanual/cover-6x9-back.jpg";
                ViewBag.Title = "Back Cover";
                ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, "Back Cover of the Live Forever Manual");

            }
            if (!string.IsNullOrEmpty(model.Description))
            {
                return View("ViewPage", "", model);
            }

            var page = _pagesProcessor.GetPageById(pageNumber);

            PdfGenerationResultViewModel onepageResultViewModel = new PdfGenerationResultViewModel();
            PathsInitialSetup();
            PdfGenerationParameters parameters = new PdfGenerationParameters(
                this.PdfRootPath, this.ImagesRootPath, this.AdditionalImagesPath, this.FontsPath, this.PdfImagesPath);

            PdfGenerator factory = new PdfGenerator(parameters);
            onepageResultViewModel = factory.GenerateOnePage(page, false);

            return RedirectToAction("Open", "ManageBook", new { fileName = onepageResultViewModel.NameOfFileResulted });
        }

        private void PathsInitialSetup()
        {
            this.PdfRootPath = Server.MapPath("~/" + PdfGen.FolderToSavePdfs);
            this.ImagesRootPath = Server.MapPath("~/Images");
            this.AdditionalImagesPath = Server.MapPath("~/Content/Images");
            this.FontsPath = Server.MapPath("~/Content/Fonts");
            this.PdfImagesPath = Server.MapPath("~/App_Data/Images/Manual");
        }

        private string GetChapterPageContent(Chapter chapterEntity, int pageNumber)
        {
            var firstChapterPage = chapterEntity.PageFirst;
            var nthChapterPage = pageNumber - firstChapterPage + 1;
            switch (nthChapterPage)
            {
                case 1: return chapterEntity.Text;
                case 2: return chapterEntity.ContentPage2;
                case 3: return chapterEntity.ContentPage3;
                case 4: return chapterEntity.ContentPage4;
                case 5: return chapterEntity.ContentPage5;
                case 6: return chapterEntity.ContentPage6;
                case 7: return chapterEntity.ContentPage7;
                case 8: return chapterEntity.ContentPage8;
                case 9: return chapterEntity.ContentPage9;
                case 10: return chapterEntity.ContentPage10;
            }

            return null;
        }

        private bool IsChapterFirstPage(Chapter chapterEntity, int pageNumber)
        {
            var firstChapterPage = chapterEntity.PageFirst;
            return firstChapterPage == pageNumber;
        }

        private string GetChapterTitle(Chapter chapterEntity)
        {
            return chapterEntity.ChapterName;
        }

        private string ReplaceImageSources(string description)
        {
            var document = new HtmlDocument();
            document.LoadHtml(description);
            document.DocumentNode.Descendants("img")
                                .Where(e =>
                                {
                                    var src = e.GetAttributeValue("src", null) ?? "";
                                    return !string.IsNullOrEmpty(src);
                                })
                                .ToList()
                                .ForEach(x =>
                                {
                                    var currentSrcValue = x.GetAttributeValue("src", null);
                                    x.SetAttributeValue("src", $"/images/liveforevermanual/chapters/{currentSrcValue}");
                                    x.AddClass("embed-responsive");
                                });
            var result = document.DocumentNode.OuterHtml;
            return result;
        }
    }
}