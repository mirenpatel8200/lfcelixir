using System;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.AdminManual.Models;
using Elixir.Areas.AdminManual.ViewModels;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models.Enums;

namespace Elixir.Areas.AdminManual.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator, Roles.Editor)]
    public class ChapterController : BaseController
    {
        private readonly IChaptersProcessor _chaptersProcessor;

        public ChapterController(IChaptersProcessor chaptersProcessor)
        {
            _chaptersProcessor = chaptersProcessor;
        }

        public ActionResult Index(ChaptersSortOrder sortBy = ChaptersSortOrder.DisplayOrder, SortDirection direction = SortDirection.Ascending)
        {
            var chapters = _chaptersProcessor.GetAllChapters(sortBy, direction).Select(x => new ChapterModel(x));

            return SortableListView<ChaptersSortOrder, ChaptersViewModel, ChapterModel>(chapters, sortBy, direction);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var chapterTypes = _chaptersProcessor.GetAllChapterTypes();
            CreateChapterViewModel viewModel = new CreateChapterViewModel(new ChapterModel()
            {
                Notes = null,
                IsIncluded = true,
                DisplayOrder = 200,
                MarginTop = 0
            }, chapterTypes);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateChapterViewModel viewModel)
        {
            viewModel.Model.ChapterTypes = _chaptersProcessor.GetAllChapterTypes();

            if (ModelState.IsValid)
            {
                _chaptersProcessor.CreateChapter(viewModel.Model);

                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                throw new ArgumentException("Incorrect id of Chapter.", nameof(id));

            var chapter = _chaptersProcessor.GetChapterById(id.Value);
            if (chapter == null)
                throw new NullReferenceException("The Chapter with specified id does not exist.");

            var chapterTypes = _chaptersProcessor.GetAllChapterTypes();
            var viewModel = new EditChapterViewModel(new ChapterModel(chapter) { ChapterTypes = chapterTypes }, chapterTypes);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditChapterViewModel viewModel)
        {
            viewModel.Model.ChapterTypes = _chaptersProcessor.GetAllChapterTypes();

            if (ModelState.IsValid)
            {
                _chaptersProcessor.UpdateChapter(viewModel.Model);

                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id.HasValue == false)
                throw new ArgumentException("Incorrect id of Chapter", nameof(id));

            _chaptersProcessor.DeleteChapter(id.Value);

            return RedirectToAction("Index");
        }
    }
}