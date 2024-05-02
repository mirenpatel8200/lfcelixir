using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.BusinessLogic.Exceptions;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class GoLinksProcessor : IGoLinksProcessor
    {
        private readonly IGoLinksRepository _goLinksRepository;

        public GoLinksProcessor(IGoLinksRepository goLinksRepository)
        {
            _goLinksRepository = goLinksRepository;
        }

        public IEnumerable<GoLink> Get100GoLinks(GoLinksSortOrder sortField, SortDirection sortDirection)
        {
            var sortFields = new[] { sortField, GoLinksSortOrder.GoLinkID };
            var sortDirections = new[] { sortDirection, SortDirection.Ascending };

            var allGoLinks = _goLinksRepository.GetN(100, sortFields, sortDirections);

            //if (sortDirection == SortDirection.Ascending)
            //{
            //    switch (sortField)
            //    {
            //        case GoLinksSortOrder.GoLinkID:
            //            allGoLinks = allGoLinks.OrderBy(x => x.Id);
            //            break;
            //        case GoLinksSortOrder.ShortCode:
            //            allGoLinks = allGoLinks.OrderBy(x => x.ShortCode).ThenBy(x => x.Id);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortField), sortField, null);
            //    }
            //}
            //else if (sortDirection == SortDirection.Descending)
            //{
            //    switch (sortField)
            //    {
            //        case GoLinksSortOrder.GoLinkID:
            //            allGoLinks = allGoLinks.OrderByDescending(x => x.Id);
            //            break;
            //        case GoLinksSortOrder.ShortCode:
            //            allGoLinks = allGoLinks.OrderByDescending(x => x.ShortCode).ThenBy(x => x.Id);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortField), sortField, null);
            //    }
            //}

            return allGoLinks;
        }


        //private void CheckExistingShortCode(GoLink goLink)
        //{
        //    var existingLink = _goLinksRepository.GetAll().FirstOrDefault(x =>
        //        x.IsDeleted == false
        //        && x.ShortCode.Equals(goLink.ShortCode, StringComparison.OrdinalIgnoreCase));

        //    if (goLink.Id.HasValue)
        //    {
        //        // Editing.
        //        if (existingLink != null && goLink.Id != existingLink.Id.Value)
        //        {
        //            throw new ArgumentException("There is a GoLink that has same ShortCode");
        //        }
        //    }
        //    else
        //    {
        //        // Adding new record.
        //        if (existingLink != null)
        //        {
        //            throw new ArgumentException("There is a GoLink that has same ShortCode");
        //        }
        //    }
        //}

        public void CreateGoLink(GoLink goLink)
        {
            //CheckExistingShortCode(goLink);
            ValidateShortCode(goLink.ShortCode);

            goLink.Created = DateTime.Now;
            _goLinksRepository.Insert(goLink);
        }

        public GoLink GetById(int id)
        {
            return _goLinksRepository.GetById(id);
        }

        public void UpdateGoLink(GoLink goLink)
        {
            //CheckExistingShortCode(goLink);
            ValidateShortCode(goLink.ShortCode, goLink.Id);

            var now = DateTime.Now;
            goLink.Updated = now;

            _goLinksRepository.Update(goLink);
        }

        public GoLink GetByShortUrl(string shortUrl)
        {
            return _goLinksRepository.GetByShortUrl(shortUrl);
        }

        public void SoftDeleteGoLink(int id)
        {
            _goLinksRepository.Delete(id);
        }

        private void ValidateShortCode(string shortCode, int? excludeGoLinkId = null)
        {
            var exists = _goLinksRepository.IsNonDeletedGoLinkExists(shortCode, excludeGoLinkId);
            if (exists)
                throw new ModelValidationException("GoLink with specified ShortCode already exists.");
        }
    }
}
