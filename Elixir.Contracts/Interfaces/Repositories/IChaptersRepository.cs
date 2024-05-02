using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IChaptersRepository : IRepository<Chapter>
    {
        IEnumerable<Chapter> GetAllChapters(ChaptersSortOrder sortOrder, SortDirection sortDirection);

        IEnumerable<Chapter> GetChaptersByType(ChapterType type, ChapterIncluded included);

        Chapter GetChapterById(int id);

        void SetChapterFirstPageLastPage(int chapterId, int firstPage, int lastPage);

        Chapter GetChapterByManualPageNumber(int pageNumber);
    }
}
