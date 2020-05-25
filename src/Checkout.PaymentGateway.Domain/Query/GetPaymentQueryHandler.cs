using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Domain.Store;
using MediatR;

namespace Checkout.PaymentGateway.Domain.Query
{
    public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, Payment>
    {
        private readonly IPaymentStore _store;

        public GetPaymentQueryHandler(IPaymentStore store)
        {
            _store = store;
        }

        public Task<Payment> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_store.Get($"{request.MerchantId}{request.PaymentId}"));
        }
    }
}   