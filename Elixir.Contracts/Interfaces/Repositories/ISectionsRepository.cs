using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface ISectionsRepository : IRepository<BookSection>
    {
        IEnumerable<BookSection> GetAllSections(BookSectionsSortOrder sortOrder, SortDirection sortDirection);
        IEnumerable<BookSection> GetIncludedSections(BookSectionsSortOrder sortOrder, SortDirection sortDirection);
        BookSection GetSectionById(int id);

        bool SectionExists(int id);

        int GetAllSectionsCount();
        int GetIncludedSectionsCount();
    }
}
