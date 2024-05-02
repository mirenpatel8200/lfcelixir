using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.AdminManual.Models;
using Elixir.Areas.AdminManual.ViewModels;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;

namespace Elixir.Areas.AdminManual.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator, Roles.Editor)]
    public class PageController : BaseController
    {
        private readonly IPagesProcessor _pagesProcessor;
        private readonly ISectionsProcessor _sectionsProcessor;

        public PageController(IPagesProcessor pagesProcessor, ISectionsProcessor sectionsProcessor)
        {
            _pagesProcessor = pagesProcessor;
            _sectionsProcessor = sectionsProcessor;
        }

        public ActionResult Index(BookPagesSortOrder sortBy = BookPagesSortOrder.BookPageName, SortDirection direction = SortDirection.Ascending)
        {
            var pages = _pagesProcessor.GetAllPages(BookDataType.All, sortBy, direction)
                .Select(x => new BookPageModel(x) {BookSection = x.BookSection});

            return SortableListView<BookPagesSortOrder, BooksViewModel, BookPageModel>(pages, sortBy, direction);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var viewModel = new EditPageViewModel();

            if (id == null)
                throw new ArgumentException("Incorrect id of Page.", nameof(id));

            var bookPage = _pagesProcessor.GetPageById(id.Value);

            if (bookPage == null)
                throw new ContentNotFoundException("The Page with specified id does not exist.");

            viewModel.Model = new BookPageModel(bookPage);

            IEnumerable<SelectListItem> selectItems;

            if (bookPage.BookSection?.Id != null)
            {
                selectItems = GetSectionsSelectItems(_sectionsProcessor.GetAllSections(BookDataType.All,
                    BookSectionsSortOrder.BookSectionName,
                    SortDirection.Ascending), bookPage.BookSection.Id.Value);
                viewModel.Model.BookSectionId = bookPage.BookSection.Id.Value;
            }
            else
            {
                selectItems = GetSectionsSelectItems(_sectionsProcessor.GetAllSections(BookDataType.All,
                    BookSectionsSortOrder.BookSectionName,
                    SortDirection.Ascending));
            }

            viewModel.SelectItems = selectItems;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditPageViewModel bookPageModel)
        {
            if (ModelState.IsValid)
            {
                bookPageModel.Model.BookSection = new BookSection() { Id = bookPageModel.Model.BookSectionId };
                _pagesProcessor.UpdatePage(bookPageModel.Model);

                return RedirectToAction("Index");
            }
            else
            {
                IEnumerable<SelectListItem> selectItems;
                if (bookPageModel.Model.BookSectionId != 0)
                {
                    selectItems = GetSectionsSelectItems(_sectionsProcessor.GetAllSections(BookDataType.All,
                        BookSectionsSortOrder.BookSectionName, SortDirection.Ascending), bookPageModel.Model.BookSectionId);
                }
                else
                {
                    selectItems = GetSectionsSelectItems(_sectionsProcessor.GetAllSections(BookDataType.All,
                        BookSectionsSortOrder.BookSectionName, SortDirection.Ascending));             
                }
                bookPageModel.SelectItems = selectItems;
                return View(bookPageModel);
            }
        }

        private IEnumerable<SelectListItem> GetSectionsSelectItems(IEnumerable<BookSection> bookSections, int selectedId = -1)
        {
            return bookSections.Select(bookSection => new SelectListItem()
            {
                Text = bookSection.BookSectionName,
                Value = bookSection.Id.ToString(),
                Selected = bookSection.Id == selectedId
            });
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new CreatePageViewModel();

            var pageModel = new BookPageModel
            {
                LifeExtension40 = 0,
                DisplayOrder = 9,
                IsIncluded = true,
                Notes = "",
                Cost = 0,
                Difficulty = 0
            };
            viewModel.SelectItems = GetSectionsSelectItems(_sectionsProcessor.GetAllSections(BookDataType.All, BookSectionsSortOrder.BookSectionName,
                SortDirection.Ascending));
            viewModel.Model = pageModel;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreatePageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Model.BookSection = new BookSection() { Id = viewModel.Model.BookSectionId };
                _pagesProcessor.CreatePage(viewModel.Model);

                return RedirectToAction("Index");
            }
            else
            {
                viewModel.SelectItems = GetSectionsSelectItems(_sectionsProcessor.GetAllSections(BookDataType.All, BookSectionsSortOrder.BookSectionName, SortDirection.Ascending));
                return View(viewModel);
            } 
        }

        public ActionResult Delete(int? id)
        {
            if (id.HasValue == false)
                throw new ArgumentException("Incorrect id of Page", nameof(id));

            _pagesProcessor.DeletePage(id.Value);

            return RedirectToAction("Index");
        }
    }
}