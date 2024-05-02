using System.Collections.Generic;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IRepository<TEntity>
    {
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(int id);

        IEnumerable<TEntity> GetAll();
    }
}
