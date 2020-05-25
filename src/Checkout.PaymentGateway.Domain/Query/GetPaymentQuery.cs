using System;
using MediatR;

namespace Checkout.PaymentGateway.Domain.Query
{
    public class GetPaymentQuery : IRequest<Payment>
    {
        public Guid PaymentId { get; }
        public string MerchantId { get; }

        public GetPaymentQuery(Guid paymentId, string merchantId)
        {
            if (paymentId == Guid.Empty)
                throw new ArgumentException($"{nameof(paymentId)} can not be empty.");

            if(string.IsNullOrWhiteSpace(merchantId))
                throw new ArgumentException($"{nameof(merchantId)} can not be null or empty.");

            PaymentId = paymentId;
            MerchantId = merchantId;
        }
    }
}
