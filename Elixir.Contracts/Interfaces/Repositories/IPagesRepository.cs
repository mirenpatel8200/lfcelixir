using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IPagesRepository : IRepository<BookPage>
    {
        IEnumerable<BookPage> GetAllPages(BookPagesSortOrder sortOrder, SortDirection sortDirection);
        IEnumerable<BookPage> GetIncludedPages(BookPagesSortOrder sortOrder, SortDirection sortDirection);

        BookPage GetPageById(int id);

        void SetPageFirstPageLastPage(int id, int firstPage, int lastPage);

        BookPage GetPageByManualPageNumber(int pageNumber);
        IEnumerable<BookPage> GetPagesBySection(int bookSectionId);
        int GetPagesCountByStatus(string status);
    }
}
