using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.BusinessLogic.Processors
{
    public class RoleProcessor : IRoleProcessor
    {
        private readonly IRoleRepository _roleRepository;

        public RoleProcessor(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
    }
}
