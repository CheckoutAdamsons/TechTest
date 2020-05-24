using System;
using Checkout.PaymentGateway.Domain.Events;

namespace Checkout.PaymentGateway.Domain
{
    public class Payment
    {
        public Guid Id { get; }
        public Guid AcquiringBankId { get; }
        public int Amount { get; }
        public string CardNumber { get; }
        public string Cvv { get; }
        public int ExpiryYear { get; }
        public int ExpiryMonth { get; }
        public Currency Currency { get; }
        public PaymentStatus Status { get; }

        public string MaskedCardNumber => CardNumber.Substring(CardNumber.Length - 4, 4).PadLeft(CardNumber.Length, 'X');

        public Payment(PaymentCreatedEvent @event)
        {
            Id = @event.Id;
            AcquiringBankId = @event.AcquiringBankId;
            Amount = @event.Amount;
            CardNumber = @event.CardNumber;
            Cvv = @event.Cvv;
            ExpiryYear = @event.ExpiryYear;
            ExpiryMonth = @event.ExpiryMonth;
            Currency = @event.Currency;
            Status = @event.Status;
        }
    }
}
