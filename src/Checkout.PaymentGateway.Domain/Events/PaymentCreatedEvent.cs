using System;
using MediatR;

namespace Checkout.PaymentGateway.Domain.Events
{
    public class PaymentCreatedEvent : INotification, IPaymentEvent
    {
        public Guid Id { get; set; }
        public Guid AcquiringBankId { get; set; }
        public int Amount { get; set; }
        public string CardNumber { get; set; }
        public string Cvv { get; set; }
        public int ExpiryYear { get; set; }
        public int ExpiryMonth { get; set; }
        public Currency Currency { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
