using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.AdminManual.Models;
using Elixir.Areas.AdminManual.ViewModels.ContentsPage;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models.Enums;

namespace Elixir.Areas.AdminManual.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator, Roles.Editor)]
    public class ContentController : BaseController
    {
        private readonly ISectionsProcessor _sectionsProcessor;
        private readonly IPagesProcessor _pagesProcessor;

        public ContentController(ISectionsProcessor sectionsProcessor, IPagesProcessor pagesProcessor)
        {
            _sectionsProcessor = sectionsProcessor;
            _pagesProcessor = pagesProcessor;
        }

        public ActionResult Index()
        {
            var viewModel = new ContentsViewModel();

            var viewModelSections = new List<SectionWithPagesModel>();

            var sections = _sectionsProcessor.GetAllSections(BookDataType.Included,
                BookSectionsSortOrder.DisplayOrder, SortDirection.Ascending).ToList();

            var totalSections = sections.Count;
            var totalPages = 0;

            var pageCounter = 1;

            foreach (var bookSection in sections)
            {
                var viewModelPages = new List<ContentsPageModel>();
                var pages = _pagesProcessor.GetPagesBySection((int)bookSection.Id).ToList();

                var sectionWithPages = new SectionWithPagesModel
                {
                    BookSection = bookSection,
                    PagesCount = pages.Count
                };

                totalPages += pages.Count;

                foreach (var bookPage in pages)
                {
                    viewModelPages.Add(new ContentsPageModel()
                    {
                        BookPage = bookPage,
                        PageNumber = pageCounter++
                    });
                }

                sectionWithPages.Pages = viewModelPages;
                viewModelSections.Add(sectionWithPages);
            }


            viewModel.TotalPages = totalPages;
            viewModel.TotalSections = totalSections;
            viewModel.DraftedPages = _pagesProcessor.GetPagesCountByStatus("Drafted");
            viewModel.TotalLe40 = _pagesProcessor.GetTotalLe40();
            viewModel.CompletePagesCount = _pagesProcessor.GetPagesCountByStatus("Complete");

            viewModel.Sections = viewModelSections;

            return View(viewModel);
        }
    }
}