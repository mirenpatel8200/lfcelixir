using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces
{
    public interface ISectionsProcessor
    {
        IEnumerable<BookSection> GetAllSections(BookDataType dataType, BookSectionsSortOrder sortOrder, SortDirection sortDirection);
        BookSection GetSectionById(int id);

        int GetSectionsCount(BookDataType dataType);

        void CreateSection(BookSection bookSection);
        void UpdateSection(BookSection newBookSection);
        void DeleteSection(int id);
    }
}
