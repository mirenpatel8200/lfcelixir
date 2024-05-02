using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces
{
    public interface IChaptersProcessor
    {
        IEnumerable<Chapter> GetAllChapters(ChaptersSortOrder sortOrder, SortDirection sortDirection);
        IEnumerable<Chapter> GetChaptersByType(ChapterType type, ChapterIncluded included);
        Chapter GetChapterById(int id);
        void CreateChapter(Chapter chapter);
        void UpdateChapter(Chapter chapter);
        void DeleteChapter(int chapterId);
        void SetChapterFirstPageLastPage(int entityId, int pageFirst, int pageLast);
        Dictionary<int, string> GetAllChapterTypes();

        Chapter GetChapterByManualPageNumber(int pageNumber);
    }
}
