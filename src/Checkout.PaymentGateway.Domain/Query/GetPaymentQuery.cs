using System;
using MediatR;

namespace Checkout.PaymentGateway.Domain.Query
{
    public class GetPaymentQuery : IRequest<Payment>
    {
        public Guid PaymentId { get; }

        public GetPaymentQuery(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException($"{nameof(paymentId)} can not be empty.");

            PaymentId = paymentId;
        }
    }
}
