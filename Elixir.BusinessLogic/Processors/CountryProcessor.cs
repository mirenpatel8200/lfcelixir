using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;
using System.Linq;
using Elixir.BusinessLogic.Core.Utils;

namespace Elixir.BusinessLogic.Processors
{
    public class CountryProcessor : ICountryProcessor
    {
        private readonly ICountryRepository _countryRepository;

        public CountryProcessor(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public IEnumerable<Country> SearchCountry(string term, int? maxCount = null)
        {
            if (string.IsNullOrEmpty(term))
                term = "";
            return _countryRepository.SearchCountry(term, maxCount);
        }
        //SearchCountryByName
        public IEnumerable<Country> SearchCountryByName(string term, int? maxCount = null)
        {
            if (string.IsNullOrEmpty(term))
                term = "";
            return _countryRepository.SearchCountryByName(term, maxCount);
        }
        public Country GetCountryById(int Id)
        {
            return _countryRepository.GetCountryById(Id);
        }

    }
}
