using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class PagesProcessor : IPagesProcessor
    {
        private IPagesRepository _pagesRepository;
        private ISectionsRepository _sectionsRepository;

        public PagesProcessor(IPagesRepository pagesRepository, ISectionsRepository sectionsRepository)
        {
            _pagesRepository = pagesRepository;
            _sectionsRepository = sectionsRepository;
        }

        public IEnumerable<BookPage> GetAllPages(BookDataType dataType, BookPagesSortOrder sortOrder, SortDirection sortDirection)
        {
            // TODO: redo.
            IEnumerable<BookPage> allBookPages;

            switch (dataType)
            {
                case BookDataType.All:
                    allBookPages = _pagesRepository.GetAllPages(sortOrder, sortDirection);
                    break;
                case BookDataType.Included:
                    allBookPages = _pagesRepository.GetIncludedPages(sortOrder, sortDirection);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
            }

            //if (sortDirection == SortDirection.Ascending)
            //{
            //    switch (sortOrder)
            //    {
            //        case BookPagesSortOrder.BookPageID:
            //            allBookPages = allBookPages.OrderBy(x => x.Id).ThenBy(x => x.BookPageName);
            //            break;
            //        case BookPagesSortOrder.BookPageName:
            //            allBookPages = allBookPages.OrderBy(x => x.BookPageName);
            //            break;
            //        case BookPagesSortOrder.DisplayOrder:
            //            allBookPages = allBookPages.OrderBy(x => x.DisplayOrder).ThenBy(x => x.BookPageName);
            //            break;
            //        case BookPagesSortOrder.IsIncluded:
            //            allBookPages = allBookPages.OrderBy(x => x.IsIncluded).ThenBy(x => x.BookPageName);
            //            break;
            //        case BookPagesSortOrder.LifeExtension40:
            //            allBookPages = allBookPages.OrderBy(x => x.LifeExtension40).ThenBy(x => x.BookPageName);
            //            break;
            //        case BookPagesSortOrder.Section:
            //            allBookPages = allBookPages.OrderBy(x => x.BookSection?.BookSectionName).ThenBy(x => x.BookPageName);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            //    }
            //}
            //else if (sortDirection == SortDirection.Descending)
            //{
            //    switch (sortOrder)
            //    {
            //        case BookPagesSortOrder.BookPageID:
            //            allBookPages = allBookPages.OrderByDescending(x => x.Id).ThenBy(x => x.BookPageName);
            //            break;
            //        case BookPagesSortOrder.BookPageName:
            //            allBookPages = allBookPages.OrderByDescending(x => x.BookPageName);
            //            break;
            //        case BookPagesSortOrder.DisplayOrder:
            //            allBookPages = allBookPages.OrderByDescending(x => x.DisplayOrder).ThenBy(x => x.BookPageName);
            //            break;
            //        case BookPagesSortOrder.IsIncluded:
            //            allBookPages = allBookPages.OrderByDescending(x => x.IsIncluded).ThenBy(x => x.BookPageName);
            //            break;
            //        case BookPagesSortOrder.LifeExtension40:
            //            allBookPages = allBookPages.OrderByDescending(x => x.LifeExtension40).ThenBy(x => x.BookPageName);
            //            break;
            //        case BookPagesSortOrder.Section:
            //            allBookPages = allBookPages.OrderByDescending(x => x.BookSection?.BookSectionName).ThenBy(x => x.BookPageName);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            //    }
            //}

            return allBookPages;
        }

        //public IEnumerable<BookPage> GetPagesIncludedAndDrafted()
        //{
        //    IEnumerable<BookPage> allBookPages;
        //    allBookPages = _pagesRepository.GetIncludedPages().Where(p => p.Status == "Drafted");

        //    return allBookPages;
        //}

        //public IEnumerable<BookPage> GetPagesBySection(BookSection section)
        //{
        //    return GetPagesBySection(section, BookDataType.All, BookPagesSortOrder.Id, SortDirection.Ascending);
        //}

        public IEnumerable<BookPage> GetPagesBySection(int bookSectionId)
        {
            return _pagesRepository.GetPagesBySection(bookSectionId);
        }

        public BookPage GetPageById(int id)
        {
            return _pagesRepository.GetPageById(id);
        }

        //public int GetTotalPagesCount(BookDataType dataType)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetPagesCountWithinSection(BookSection bookSection, BookDataType dataType)
        //{
        //    throw new NotImplementedException();
        //}

        public void CreatePage(BookPage bookPage)
        {
            if (bookPage.BookSection == null || bookPage.BookSection.Id.HasValue == false || _sectionsRepository.SectionExists(bookPage.BookSection.Id.Value) == false)
                throw new NullReferenceException("Page does not have the Section.");

            if (String.IsNullOrEmpty(bookPage.Notes))
                bookPage.Notes = "";

            _pagesRepository.Insert(bookPage);
        }

        public void UpdatePage(BookPage bookPage)
        {
            if (bookPage.BookSection == null || bookPage.BookSection.Id.HasValue == false || _sectionsRepository.SectionExists(bookPage.BookSection.Id.Value) == false)
                throw new NullReferenceException("Page does not have the Section.");

            if (bookPage.Notes == null)
                bookPage.Notes = "";

            //bool sectionExists = _sectionsRepository.SectionExists(bookPage.BookSection.Id.Value);
            //if (sectionExists == false)
            //    throw new InvalidOperationException("Specified section for Page does not exist.");

            _pagesRepository.Update(bookPage);
        }

        public void DeletePage(int id)
        {
            _pagesRepository.Delete(id);
        }

        public int GetPagesCountByStatus(string status)
        {
            return _pagesRepository.GetPagesCountByStatus(status);
            //return _pagesRepository.GetAllPages().Count(x => x.Status?.Equals(status) ?? false);
        }

        public int GetTotalLe40()
        {
            return _pagesRepository.GetIncludedPages(BookPagesSortOrder.DisplayOrder, SortDirection.Ascending).Aggregate(0, (i, page) => i += page.LifeExtension40);
            //return _pagesRepository.GetAllPages().Where(x => x.IsIncluded).Aggregate(0, (i, page) => i += page.LifeExtension40);
        }

        public void SetPageFirstPageLastPage(int entityId, int pageFirst, int pageLast)
        {
            _pagesRepository.SetPageFirstPageLastPage(entityId, pageFirst, pageLast);
        }

        public BookPage GetPageByManualPageNumber(int pageNumber)
        {
            return _pagesRepository.GetPageByManualPageNumber(pageNumber);
        }
    }
}
