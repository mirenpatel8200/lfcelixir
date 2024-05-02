using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.BusinessLogic.Processors
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentProcessor(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public void CreatePayment(Payments entity)
        {
            entity.CreatedOn = DateTime.Now;
            entity.UpdatedOn = DateTime.Now;
            _paymentRepository.Insert(entity);

            var addedPayment = _paymentRepository.GetPaymentByPaymentReference(entity.PaymentReference);
            if (addedPayment == null)
                throw new InvalidOperationException("Unable to find payment that was added.");
        }

        public void UpdatePayment(Payments entity)
        {
            entity.UpdatedOn = DateTime.Now;
            _paymentRepository.Update(entity);
        }

        public IEnumerable<Payments> GetPaymentsByShopOrder(int shopOrderId)
        {
            return _paymentRepository.GetPaymentsByShopOrder(shopOrderId);
        }
    }
}
