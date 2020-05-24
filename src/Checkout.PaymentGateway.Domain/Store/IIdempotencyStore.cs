namespace Checkout.PaymentGateway.Domain.Store
{
    public interface IIdempotencyStore<T>
    {
        public bool Add(string key, T item);

        public bool Update(string key, T item);

        public bool Remove(string key);

        public bool Get(string key, out T value);
    }
}