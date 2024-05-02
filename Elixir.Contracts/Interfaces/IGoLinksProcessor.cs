using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces
{
    public interface IGoLinksProcessor
    {
        IEnumerable<GoLink> Get100GoLinks(GoLinksSortOrder sortField, SortDirection sortDirection);
        void CreateGoLink(GoLink goLink);
        GoLink GetById(int id);
        void UpdateGoLink(GoLink goLink);
        GoLink GetByShortUrl(string shortUrl);
        void SoftDeleteGoLink(int id);
    }
}
