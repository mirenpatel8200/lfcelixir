using System;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.AdminManual.Models;
using Elixir.Areas.AdminManual.ViewModels;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;

namespace Elixir.Areas.AdminManual.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator, Roles.Editor)]
    public class SectionController : BaseController
    {
        private readonly ISectionsProcessor _sectionsProcessor;

        public SectionController(ISectionsProcessor sectionsProcessor)
        {
            _sectionsProcessor = sectionsProcessor;
        }

        // GET: Sections
        public ActionResult Index(int sortBy = 0, int direction = 0)
        {
            var viewModel = new SectionsViewModel();

            var sortDirection = (SortDirection)direction;
            var sortOrder = (BookSectionsSortOrder)sortBy;

            var newSortDirection = SortDirection.Descending;

            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    newSortDirection = SortDirection.Descending;
                    break;
                case SortDirection.Descending:
                    newSortDirection = SortDirection.Ascending;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            viewModel.SortDirection = newSortDirection;
            viewModel.SortOrder = sortOrder;

            viewModel.Models = _sectionsProcessor.GetAllSections(BookDataType.All, sortOrder, sortDirection).Select(x => new BookSectionModel(x));

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var section = new BookSectionModel();
            section.DisplayOrder = 9;
            section.IsIncluded = true;

            return View(section);
        }

        [HttpPost]
        public ActionResult Create(BookSectionModel bookSection)
        {
            if (ModelState.IsValid)
            {
                _sectionsProcessor.CreateSection(bookSection);

                return RedirectToAction("Index");
            }

            return View(bookSection);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            BookSectionModel section = null;

            if (id.HasValue == false)
                throw new ArgumentException("Incorrect id of Section.", nameof(id));

            var bookSection = _sectionsProcessor.GetSectionById(id.Value);

            section = new BookSectionModel(bookSection);

            if (section == null)
                throw new ContentNotFoundException("The Section with specified id does not exist.");

            return View(section);
        }

        [HttpPost]
        public ActionResult Edit(BookSectionModel bookSection)
        {
            if (ModelState.IsValid)
            {
                _sectionsProcessor.UpdateSection(bookSection);

                return RedirectToAction("Index");
            }

            return View(bookSection);
        }

        public ActionResult Delete(int? id)
        {
            if (id.HasValue == false)
                throw new ArgumentException("Incorrect id of Section", nameof(id));

            _sectionsProcessor.DeleteSection(id.Value);

            return RedirectToAction("Index");
        }
    }
}