using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class ChaptersProcessor : IChaptersProcessor
    {
        private readonly IChaptersRepository _chaptersRepository;

        public ChaptersProcessor(IChaptersRepository chaptersRepository)
        {
            _chaptersRepository = chaptersRepository;
        }

        public IEnumerable<Chapter> GetAllChapters(ChaptersSortOrder sortOrder, SortDirection sortDirection)
        {
            return _chaptersRepository.GetAllChapters(sortOrder, sortDirection);
            //IEnumerable<Chapter> allChapters = _chaptersRepository.GetAllChapters();

            //if (sortDirection == SortDirection.Ascending)
            //{
            //    switch (sortOrder)
            //    {
            //        case ChaptersSortOrder.BookChapterID:
            //            allChapters = allChapters.OrderBy(x => x.Id).ThenBy(x => x.ChapterName);
            //            break;
            //        case ChaptersSortOrder.BookChapterName:
            //            allChapters = allChapters.OrderBy(x => x.ChapterName);
            //            break;
            //        case ChaptersSortOrder.DisplayOrder:
            //            allChapters = allChapters.OrderBy(x => x.DisplayOrder).ThenBy(x => x.ChapterName);
            //            break;
            //        case ChaptersSortOrder.IsIncluded:
            //            allChapters = allChapters.OrderBy(x => x.IsIncluded).ThenBy(x => x.ChapterName);
            //            break;
            //        case ChaptersSortOrder.BookChapterTypeID:
            //            allChapters = allChapters.OrderBy(x => x.TypeID).ThenBy(x => x.ChapterName);
            //            break;

            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            //    }
            //}
            //else if (sortDirection == SortDirection.Descending)
            //{
            //    switch (sortOrder)
            //    {
            //        case ChaptersSortOrder.BookChapterID:
            //            allChapters = allChapters.OrderByDescending(x => x.Id).ThenBy(x => x.ChapterName);
            //            break;
            //        case ChaptersSortOrder.BookChapterName:
            //            allChapters = allChapters.OrderByDescending(x => x.ChapterName);
            //            break;
            //        case ChaptersSortOrder.DisplayOrder:
            //            allChapters = allChapters.OrderByDescending(x => x.DisplayOrder).ThenBy(x => x.ChapterName);
            //            break;
            //        case ChaptersSortOrder.IsIncluded:
            //            allChapters = allChapters.OrderByDescending(x => x.IsIncluded).ThenBy(x => x.ChapterName);
            //            break;
            //        case ChaptersSortOrder.BookChapterTypeID:
            //            allChapters = allChapters.OrderByDescending(x => x.TypeID).ThenBy(x => x.ChapterName);
            //            break;

            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            //    }
            //}

            //return allChapters;
        }

        public Dictionary<int, string> GetAllChapterTypes()
        {
            Dictionary<int, string> chapterTypes = new Dictionary<int, string>
            {
                { 20, "Body" },
                { 10, "Front matter"},
                { 30, "Back matter"}
            };

            return chapterTypes;
        }

        public void CreateChapter(Chapter chapter)
        {
            _chaptersRepository.Insert(chapter);
        }

        public Chapter GetChapterById(int id)
        {
            return _chaptersRepository.GetChapterById(id);
        }

        public void UpdateChapter(Chapter chapter)
        {
            _chaptersRepository.Update(chapter);
        }

        public void DeleteChapter(int chapterId)
        {
            _chaptersRepository.Delete(chapterId);
        }

        public IEnumerable<Chapter> GetChaptersByType(ChapterType type, ChapterIncluded included)
        {
            return _chaptersRepository.GetChaptersByType(type, included);
        }

        public void SetChapterFirstPageLastPage(int entityId, int pageFirst, int pageLast)
        {
            _chaptersRepository.SetChapterFirstPageLastPage(entityId, pageFirst, pageLast);
        }

        public Chapter GetChapterByManualPageNumber(int pageNumber)
        {
            return _chaptersRepository.GetChapterByManualPageNumber(pageNumber);
        }
    }
}
