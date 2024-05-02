using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces
{
    public interface IPagesProcessor
    {
        IEnumerable<BookPage> GetAllPages(BookDataType dataType, BookPagesSortOrder sortOrder, SortDirection sortDirection);
        //IEnumerable<BookPage> GetPagesBySection(BookSection section);
        IEnumerable<BookPage> GetPagesBySection(int bookSectionId);
        //IEnumerable<BookPage> GetPagesIncludedAndDrafted();
        BookPage GetPageById(int id);

        //int GetTotalPagesCount(BookDataType dataType);
        //int GetPagesCountWithinSection(BookSection bookSection, BookDataType dataType);

        void CreatePage(BookPage bookPage);
        void UpdatePage(BookPage bookPage);
        void DeletePage(int id);
        int GetPagesCountByStatus(string status);
        int GetTotalLe40();
        void SetPageFirstPageLastPage(int entityId, int pageFirst, int pageLast);

        BookPage GetPageByManualPageNumber(int pageNumber);
    }
}
