using System;

namespace Checkout.PaymentGateway.AcquiringBankSimulator
{
    public class PaymentResponse
    {
        public Guid Id { get; private set; }
        public Status Status { get; private set; }

        public PaymentResponse(Status status)
        {
            Id = Guid.NewGuid();
            Status = status;
        }
    }
}