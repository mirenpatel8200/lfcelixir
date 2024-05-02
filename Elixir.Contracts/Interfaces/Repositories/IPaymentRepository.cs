using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IPaymentRepository : IRepository<Payments>
    {
        IEnumerable<Payments> GetPaymentsByShopOrder(int shopOrderId);
        Payments GetPaymentByPaymentReference(string paymentReference);
    }
}
