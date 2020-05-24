namespace Checkout.PaymentGateway.Domain.Infrastructure
{
    public interface IIdempotentRequest<T>
    {
        public string IdempotencyKey { get; }
    }
}