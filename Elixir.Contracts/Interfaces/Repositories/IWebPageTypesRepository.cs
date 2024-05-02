using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IWebPageTypesRepository : IRepository<WebPageType>
    {
        //WebPageType GetType(int id);
    }
}
