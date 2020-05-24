using System;
using Checkout.PaymentGateway.Domain;

namespace Checkout.PaymentGateway.Api.V1
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public string CardNumber { get; set; }
        public int? ExpiryMonth { get; set; }
        public int? ExpiryYear { get; set; }
        public Currency Currency { get; set; }
        public PaymentStatus Status { get; set; }

        public PaymentResponse() { }

        public PaymentResponse(Payment payment)
        {
            Id = payment.Id;
            Amount = payment.Amount;
            CardNumber = payment.MaskedCardNumber;
            ExpiryMonth = payment.ExpiryMonth;
            ExpiryYear = payment.ExpiryYear;
            Currency = payment.Currency;
            Status = payment.Status;
        }
    }
}
