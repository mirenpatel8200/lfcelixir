using Elixir.Models;
using System.Collections.Generic;
using Elixir.Models.Enums;
using System;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface ICountryRepository : IRepository<Country>
    {
        IEnumerable<Country> SearchCountry(string term, int? maxCount = null);
        IEnumerable<Country> SearchCountryByName(string term, int? maxCount = null);
        Country GetCountryById(int Id);

    }
}
