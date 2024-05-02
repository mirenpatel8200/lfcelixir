using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces
{
    public interface IPaymentProcessor
    {
        void CreatePayment(Payments entity);
        void UpdatePayment(Payments entity);
        IEnumerable<Payments> GetPaymentsByShopOrder(int shopOrderId);
    }
}
