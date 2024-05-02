using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IWebPagesRepository : IRepository<WebPage>
    {
        //IEnumerable<WebPage> GetAllWebPages();
        IEnumerable<WebPage> GetAllShopWebPages();

        WebPage GetWebPageById(int id);

        WebPage GetWebPageByUrlName(string urlName, int webPageTypeId);
        IEnumerable<WebPage> GetWebPagesChildren(int parentId);
        bool UrlNameExists(string urlName, int webPageTypeId, int? excludeId);

        int GetWebPageType(int id);

        IEnumerable<WebPage> SearchSQL(List<string> terms, bool all);
        IEnumerable<WebPage> GetNFilter(int count, bool includeDeleted, string filter, int pageTypeId, WebPagesSortOrder sortOrder, SortDirection sortDirection);
        void DeleteWebPage(int id);
        IEnumerable<WebPage> GetWebPageSiblings(int webPageId);
        IEnumerable<WebPage> GetAllWebPagesForSiteMapGenerator();
    }
}
