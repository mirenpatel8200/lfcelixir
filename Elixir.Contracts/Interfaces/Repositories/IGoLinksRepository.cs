using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IGoLinksRepository : IRepository<GoLink>
    {
        IEnumerable<GoLink> GetN(int count, GoLinksSortOrder[] sortFields, SortDirection[] sortDirections);
        GoLink GetByShortUrl(string shortUrl);
        GoLink GetById(int id);
        bool IsNonDeletedGoLinkExists(string shortCode, int? excludeGoLinkId);
    }
}
