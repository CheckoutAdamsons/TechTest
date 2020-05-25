using Checkout.PaymentGateway.Domain.Events;

namespace Checkout.PaymentGateway.Domain.Store
{
    public interface IPaymentStore
    {
        void Append(IPaymentEvent payment);
        Payment Get(string key);
    }
}
