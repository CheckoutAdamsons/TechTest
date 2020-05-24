using System;

namespace Checkout.PaymentGateway.Domain.Events
{
    public interface IPaymentEvent
    {
        public Guid Id { get; set; }
    }
}
