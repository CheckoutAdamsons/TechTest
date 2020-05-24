using System;
using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Domain.Store;
using MediatR;

namespace Checkout.PaymentGateway.Domain.Events
{
    public class PaymentCreatedEventHandler : INotificationHandler<PaymentCreatedEvent>
    {
        private readonly IPaymentStore _paymentStore;

        public PaymentCreatedEventHandler(IPaymentStore paymentStore)
        {
            _paymentStore = paymentStore ?? throw new ArgumentNullException(nameof(paymentStore));
        }

        public Task Handle(PaymentCreatedEvent notification, CancellationToken cancellationToken)
        {
            _paymentStore.Append(notification);

            return Task.CompletedTask;
        }
    }
}
