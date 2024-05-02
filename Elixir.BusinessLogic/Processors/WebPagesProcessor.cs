using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.BusinessLogic.Exceptions;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class WebPagesProcessor : IWebPagesProcessor
    {
        private readonly IWebPagesRepository _webPagesRepository;

        public WebPagesProcessor(IWebPagesRepository repository)
        {
            _webPagesRepository = repository;
        }

        public void CreateWebPage(WebPage webpage)
        {
            CheckUrlName(webpage.UrlName, webpage.TypeID);
            CheckIfPageTypeMatchesParentType(webpage.TypeID, webpage.ParentID);

            var now = DateTime.Now;
            webpage.CreatedDateTime = now;
            webpage.UpdatedDateTime = now;

            webpage.PublishedUpdatedDT = webpage.IsEnabled ? webpage.UpdatedDateTime : null;

            _webPagesRepository.Insert(webpage);
        }

        public void DeleteWebPage(int id)
        {
            _webPagesRepository.DeleteWebPage(id);
        }

        public IEnumerable<WebPage> GetWebPageSiblings(int webPageId)
        {
            return _webPagesRepository.GetWebPageSiblings(webPageId);
        }

        public IEnumerable<WebPage> GetAllWebPages()
        {
            return _webPagesRepository.GetAll();
        }

        public IEnumerable<WebPage> GetAllShopWebPages()
        {
            return _webPagesRepository.GetAllShopWebPages();
        }

        public IEnumerable<WebPage> GetNFilteredWebPages(int count, bool includeDeleted, string filter, int pageTypeId, WebPagesSortOrder sortOrder, SortDirection sortDirection)
        {
            return _webPagesRepository.GetNFilter(count, includeDeleted, filter, pageTypeId, sortOrder, sortDirection);
        }

        public WebPage GetWebPageById(int id)
        {
            return _webPagesRepository.GetWebPageById(id);
        }

        public WebPage GetWebPageByUrlName(string name, int webPageTypeId)
        {
            return _webPagesRepository.GetWebPageByUrlName(name, webPageTypeId);
        }

        private void CheckUrlName(string urlName, int webPageTypeId, int? excludeId = null)
        {
            var exists = _webPagesRepository.UrlNameExists(urlName, webPageTypeId, excludeId);
            if (exists)
                throw new ModelValidationException("Web page with specified URL name already exists.");
        }

        public void UpdateWebPage(WebPage webpage, bool isSignificantChange = false)
        {
            CheckUrlName(webpage.UrlName, webpage.TypeID, webpage.Id.Value);
            CheckIfPageTypeMatchesParentType(webpage.TypeID, webpage.ParentID);
            var prevState = GetWebPageById(webpage.Id.Value);
            var now = DateTime.Now;
            webpage.UpdatedDateTime = now;

            if (!isSignificantChange)
            {
                webpage.PublishedUpdatedDT = prevState.PublishedUpdatedDT;
            }

            if (webpage.IsEnabled)
            {
                if (prevState.PublishedUpdatedDT.HasValue && isSignificantChange ||
                    prevState.PublishedUpdatedDT.HasValue == false)
                {
                    webpage.PublishedUpdatedDT = now;
                }
                else
                {
                    webpage.PublishedUpdatedDT = prevState.PublishedUpdatedDT;
                }
            }
            else
            {
                webpage.PublishedUpdatedDT = prevState.PublishedUpdatedDT;
            }
            _webPagesRepository.Update(webpage);
        }

        public IEnumerable<WebPage> GetWebPagesChildren(int parentId)
        {
            return _webPagesRepository.GetWebPagesChildren(parentId);
        }

        //public IEnumerable<WebPage> GetAllWebPages(WebPagesSortOrder sortOrder, SortDirection sortDirection, bool includeDeleted = false)
        //{
        //    var webPages = GetAllWebPages(includeDeleted);

        //    switch (sortOrder)
        //    {
        //        case WebPagesSortOrder.WebPageID:
        //            {
        //                if (sortDirection == SortDirection.Ascending)
        //                    webPages = webPages.OrderBy(wp => wp.Id);
        //                else
        //                    webPages = webPages.OrderByDescending(wp => wp.Id);
        //                break;
        //            }
        //        case WebPagesSortOrder.WebPageName:
        //            {
        //                if (sortDirection == SortDirection.Ascending)
        //                    webPages = webPages.OrderBy(wp => wp.WebPageName);
        //                else
        //                    webPages = webPages.OrderByDescending(wp => wp.WebPageName);
        //                break;
        //            }
        //        case WebPagesSortOrder.IsEnabled:
        //            {
        //                if (sortDirection == SortDirection.Ascending)
        //                    webPages = webPages.OrderBy(wp => wp.IsEnabled);
        //                else
        //                    webPages = webPages.OrderByDescending(wp => wp.IsEnabled);
        //                break;
        //            }

        //        case WebPagesSortOrder.ParentName:
        //            {
        //                if (sortDirection == SortDirection.Ascending)
        //                    webPages = webPages.OrderBy(wp => wp.ParentName);
        //                else
        //                    webPages = webPages.OrderByDescending(wp => wp.ParentName);
        //                break;
        //            }
        //        case WebPagesSortOrder.UpdatedDT:
        //            {
        //                if (sortDirection == SortDirection.Ascending)
        //                    webPages = webPages.OrderBy(wp => wp.UpdatedDateTime);
        //                else
        //                    webPages = webPages.OrderByDescending(wp => wp.UpdatedDateTime);
        //                break;
        //            }
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
        //    }

        //    return webPages;
        //}

        //public IEnumerable<WebPage> Get100FilteredWebPages(WebPagesSortOrder sortOrder, SortDirection sortDirection,
        //    bool includeDeleted = false, string filter = "", int pageTypeId = 0)
        //{
        //    var noOfWebPages = 100;
        //    return GetNFilteredWebPages(noOfWebPages, false, filter, pageTypeId, sortOrder, sortDirection);

        //    //commented By Harmeet: Not Sorting is At SQL level
        //    //switch (sortOrder)
        //    //{
        //    //    case WebPagesSortOrder.WebPageID:
        //    //        {
        //    //            if (sortDirection == SortDirection.Ascending)
        //    //                webPages = webPages.OrderBy(wp => wp.Id);
        //    //            else
        //    //                webPages = webPages.OrderByDescending(wp => wp.Id);
        //    //            break;
        //    //        }
        //    //    case WebPagesSortOrder.Name:
        //    //        {
        //    //            if (sortDirection == SortDirection.Ascending)
        //    //                webPages = webPages.OrderBy(wp => wp.WebPageName);
        //    //            else
        //    //                webPages = webPages.OrderByDescending(wp => wp.WebPageName);
        //    //            break;
        //    //        }
        //    //    case WebPagesSortOrder.IsEnabled:
        //    //        {
        //    //            if (sortDirection == SortDirection.Ascending)
        //    //                webPages = webPages.OrderBy(wp => wp.IsEnabled);
        //    //            else
        //    //                webPages = webPages.OrderByDescending(wp => wp.IsEnabled);
        //    //            break;
        //    //        }

        //    //    case WebPagesSortOrder.ParentName:
        //    //        {
        //    //            if (sortDirection == SortDirection.Ascending)
        //    //                webPages = webPages.OrderBy(wp => wp.ParentName);
        //    //            else
        //    //                webPages = webPages.OrderByDescending(wp => wp.ParentName);
        //    //            break;
        //    //        }
        //    //    case WebPagesSortOrder.Updated:
        //    //        {
        //    //            if (sortDirection == SortDirection.Ascending)
        //    //                webPages = webPages.OrderBy(wp => wp.UpdatedDateTime);
        //    //            else
        //    //                webPages = webPages.OrderByDescending(wp => wp.UpdatedDateTime);
        //    //            break;
        //    //        }
        //    //    default:
        //    //        throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
        //    //}
        //}

        private void CheckIfPageTypeMatchesParentType(int childTypeId, int? parentPageId)
        {
            if (parentPageId.HasValue)
            {
                var parentTypeId = _webPagesRepository.GetWebPageType(parentPageId.Value);
                if (parentTypeId != childTypeId)
                {
                    throw new ModelValidationException("Parent type does not match page Type.");
                }
            }
        }

        public IEnumerable<WebPage> Search(List<string> terms, bool all)
        {
            return _webPagesRepository.SearchSQL(terms, all);
        }

    }
}
