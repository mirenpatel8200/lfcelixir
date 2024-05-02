using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class RoleRepository : AbstractRepository<Role>, IRoleRepository
    {
        public override void Insert(Role entity)
        {
            throw new NotImplementedException();
        }

        public override void Update(Role entity)
        {
            throw new NotImplementedException();
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Role> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
