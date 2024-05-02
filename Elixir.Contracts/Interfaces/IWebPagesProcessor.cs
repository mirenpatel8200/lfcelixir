using System;
using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces
{
    public interface IWebPagesProcessor
    {
        IEnumerable<WebPage> GetAllWebPages();
        IEnumerable<WebPage> GetAllShopWebPages();

        //[Obsolete("Sorting should be done in View project, not in BL - AY's fault.")]
        //IEnumerable<WebPage> GetAllWebPages(WebPagesSortOrder sortOrder, SortDirection sortDirection, bool includeDeleted = false);

        WebPage GetWebPageById(int id);

        WebPage GetWebPageByUrlName(string name, int webPageTypeId);

        IEnumerable<WebPage> GetWebPagesChildren(int parentId);

        void CreateWebPage(WebPage webPage);

        void UpdateWebPage(WebPage webPage, bool isSignificantChange);

        void DeleteWebPage(int id);

        /// <summary>
        /// Returns related pages which are not deleted an enabled.
        /// </summary>
        /// <param name="webPageId"></param>
        /// <returns></returns>
        IEnumerable<WebPage> GetWebPageSiblings(int webPageId);

        IEnumerable<WebPage> Search(List<string> terms, bool all);

        IEnumerable<WebPage> GetNFilteredWebPages(int count, bool includeDeleted, string filter, int pageTypeId, WebPagesSortOrder sortOrder, SortDirection sortDirection);

        //IEnumerable<WebPage> Get100FilteredWebPages(WebPagesSortOrder sortOrder, SortDirection sortDirection,
        //    bool includeDeleted = false, string filter = "", int pageTypeId = 0);
    }
}
