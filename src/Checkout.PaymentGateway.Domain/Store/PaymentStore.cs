using System;
using System.Collections.Concurrent;
using System.Linq;
using Checkout.PaymentGateway.Domain.Events;

namespace Checkout.PaymentGateway.Domain.Store
{
    /// <summary>
    /// Simulates a store for payment events
    /// </summary>  
    public class PaymentStore : IPaymentStore
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<IPaymentEvent>> _store = new ConcurrentDictionary<string, ConcurrentBag<IPaymentEvent>>();

        public void Append(IPaymentEvent @event)
        {
            var key = @event.Id.ToString();

            if (_store.TryGetValue(key, out var eventCollection))
            {
                eventCollection.Add(@event);
            }
            else
            {
                _store[key] = new ConcurrentBag<IPaymentEvent> { @event };
            }
        }

        public Payment Get(Guid paymentId)
        {
            var key = paymentId.ToString();

            if (_store.TryGetValue(key, out var events))
            {
                // This is a very simplified version of rebuilding the domain model from stored events
                // This type combines an event store and a payment service type, this should really be split up, but have been combined for brevity.
                var createdEvent = events.FirstOrDefault(o => o is PaymentCreatedEvent);
                return new Payment(createdEvent as PaymentCreatedEvent);
            }

            return default;
        }
    }
}
