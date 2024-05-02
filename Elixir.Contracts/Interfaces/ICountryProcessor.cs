using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;

namespace Elixir.Contracts.Interfaces
{
    public interface ICountryProcessor
    {
        IEnumerable<Country> SearchCountry(string term, int? maxCount = null);
        IEnumerable<Country> SearchCountryByName(string term, int? maxCount = null);
        Country GetCountryById(int Id);
    }
}
