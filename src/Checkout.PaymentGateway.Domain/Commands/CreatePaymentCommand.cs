using System;
using Checkout.PaymentGateway.Domain.Infrastructure;
using MediatR;

namespace Checkout.PaymentGateway.Domain.Commands
{
    public class CreatePaymentCommand : IRequest<CommandResponse>, IIdempotentRequest<CommandResponse>
    {
        public Guid Id { get; }
        public string MerchantId { get; }
        public int Amount { get; }
        public string CardNumber { get; }
        public int ExpiryMonth { get; }
        public int ExpiryYear { get; }
        public string Cvv { get; }
        public Currency Currency { get; }

        public CreatePaymentCommand(Guid id, string merchantId, int amount, string cardNumber, int expiryMonth, int expiryYear, string cvv, Currency currency)
        {
            if (string.IsNullOrWhiteSpace(merchantId)) throw new ArgumentException(nameof(merchantId));
            if (string.IsNullOrWhiteSpace(cardNumber)) throw new ArgumentException(nameof(cardNumber));
            if (string.IsNullOrWhiteSpace(cvv)) throw new ArgumentException(nameof(cvv));
            if (!Enum.IsDefined(typeof(Currency), currency)) throw new ArgumentException(nameof(currency));

            Id = id;
            MerchantId = merchantId;
            Amount = amount;
            CardNumber = cardNumber;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            Cvv = cvv;
            Currency = currency;
        }

        public string IdempotencyKey => MerchantId + Id;
    }
}
