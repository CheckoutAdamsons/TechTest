using System;

namespace Checkout.PaymentGateway.Domain.Clients
{
    public class AcquiringBankPaymentResponse
    {
        public Guid Id { get; set; }
        public AcquiringBankPaymentStatus Status { get; set; }
    }
}
