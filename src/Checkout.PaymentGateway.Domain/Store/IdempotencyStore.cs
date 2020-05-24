using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Checkout.PaymentGateway.Domain.Store
{
    public class IdempotencyStore<T> : IIdempotencyStore<T>
    {
        // This would be replaced with an external service e.g. redis which could also cover key expiry (which is not currently covered)
        private static readonly ConcurrentDictionary<string, T> Store = new ConcurrentDictionary<string, T>();

        public bool Add(string key, T value) => Store.TryAdd(key, value);

        public bool Update(string key, T value) => Store.TryUpdate(key, value, default);

        public bool Remove(string key) => Store.Remove(key, out _);

        public bool Get(string key, out T value) => Store.TryGetValue(key, out value);
    }
}