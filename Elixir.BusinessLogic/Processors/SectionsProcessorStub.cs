using System;
using System.Collections.Generic;
using Elixir.Contracts.Interfaces;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class SectionsProcessorStub : ISectionsProcessor
    {
        public IEnumerable<BookSection> GetAllSections(BookDataType dataType, BookSectionsSortOrder sortOrder,
            SortDirection sortDirection)
        {
            return new BookSection[]
            {
                new BookSection("Section A")
                {
                    Id = 1,
                    IsIncluded = false,
                    DisplayOrder = 10
                },
                new BookSection("Section B")
                {
                    Id = 2,
                    IsIncluded = false,
                    DisplayOrder = 9
                },
                new BookSection("Section C")
                {
                    Id = 3,
                    IsIncluded = true,
                    DisplayOrder = 11
                },
                new BookSection("Section D")
                {
                    Id = 4,
                    IsIncluded = false,
                    DisplayOrder = 8
                },
            };
        }

        public BookSection GetSectionById(int id)
        {
            return new BookSection("Test Section for Editing")
            {
                Id = id,
                IsIncluded = true,
                DisplayOrder = 15
            };
        }

        public int GetSectionsCount(BookDataType dataType)
        {
            throw new NotImplementedException();
        }

        public void CreateSection(BookSection bookSection)
        {
            throw new NotImplementedException();
        }

        public void UpdateSection(BookSection newBookSection)
        {
            throw new NotImplementedException();
        }

        public void DeleteSection(int id)
        {
            throw new NotImplementedException();
        }
    }
}
