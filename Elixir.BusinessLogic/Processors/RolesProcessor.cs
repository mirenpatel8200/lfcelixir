using System.Collections.Generic;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;

namespace Elixir.BusinessLogic.Processors
{
    public class RolesProcessor : IRolesProcessor
    {
        private IUserRolesRepository _rolesRepository;

        public RolesProcessor(IUserRolesRepository rolesRepository)
        {
            _rolesRepository = rolesRepository;
        }

        //public IEnumerable<BookUserRole> GetAllRoles()
        //{
        //    return _rolesRepository.GetAll();
        //}
    }
}
