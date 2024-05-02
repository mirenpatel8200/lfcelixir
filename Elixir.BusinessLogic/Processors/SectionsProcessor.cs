using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class SectionsProcessor : ISectionsProcessor
    {
        private ISectionsRepository _sectionsRepository;

        public SectionsProcessor(ISectionsRepository sectionsRepository)
        {
            _sectionsRepository = sectionsRepository;
        }

        public IEnumerable<BookSection> GetAllSections(BookDataType dataType, BookSectionsSortOrder sortOrder, SortDirection sortDirection)
        {
            IEnumerable<BookSection> allBookSections;

            switch (dataType)
            {
                case BookDataType.All:
                    allBookSections = _sectionsRepository.GetAllSections(sortOrder, sortDirection);
                    break;
                case BookDataType.Included:
                    allBookSections = _sectionsRepository.GetIncludedSections(sortOrder, sortDirection);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
            }

            //if (sortDirection == SortDirection.Ascending)
            //{
            //    switch (sortOrder)
            //    {
            //        case BookSectionsSortOrder.BookSectionID:
            //            allBookSections = allBookSections.OrderBy(x => x.Id).ThenBy(x => x.BookSectionName);
            //            break;
            //        case BookSectionsSortOrder.BookSectionName:
            //            allBookSections = allBookSections.OrderBy(x => x.BookSectionName);
            //            break;
            //        case BookSectionsSortOrder.DisplayOrder:
            //            allBookSections = allBookSections.OrderBy(x => x.DisplayOrder).ThenBy(x => x.BookSectionName);
            //            break;
            //        case BookSectionsSortOrder.IsIncluded:
            //            allBookSections = allBookSections.OrderBy(x => x.IsIncluded).ThenBy(x => x.BookSectionName);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            //    }
            //}
            //else if (sortDirection == SortDirection.Descending)
            //{
            //    switch (sortOrder)
            //    {
            //        case BookSectionsSortOrder.BookSectionID:
            //            allBookSections = allBookSections.OrderByDescending(x => x.Id).ThenBy(x => x.BookSectionName);
            //            break;
            //        case BookSectionsSortOrder.BookSectionName:
            //            allBookSections = allBookSections.OrderByDescending(x => x.BookSectionName);
            //            break;
            //        case BookSectionsSortOrder.DisplayOrder:
            //            allBookSections = allBookSections.OrderByDescending(x => x.DisplayOrder).ThenBy(x => x.BookSectionName);
            //            break;
            //        case BookSectionsSortOrder.IsIncluded:
            //            allBookSections = allBookSections.OrderByDescending(x => x.IsIncluded).ThenBy(x => x.BookSectionName);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            //    }
            //}

            return allBookSections;
        }

        public BookSection GetSectionById(int id)
        {
            return _sectionsRepository.GetSectionById(id);
        }

        public void DeleteSection(int id)
        {
            _sectionsRepository.Delete(id);
        }

        public void UpdateSection(BookSection newBookSection)
        {
            _sectionsRepository.Update(newBookSection);
        }

        public void CreateSection(BookSection bookSection)
        {
            _sectionsRepository.Insert(bookSection);
        }

        public int GetSectionsCount(BookDataType dataType)
        {
            int count;

            switch (dataType)
            {
                case BookDataType.All:
                    count = _sectionsRepository.GetAllSectionsCount();
                    break;
                case BookDataType.Included:
                    count = _sectionsRepository.GetIncludedSectionsCount();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
            }

            return count;
        }
    }
}
