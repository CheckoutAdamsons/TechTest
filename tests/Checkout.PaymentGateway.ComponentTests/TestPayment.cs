using System;
using Checkout.PaymentGateway.Api.V1;
using Checkout.PaymentGateway.Domain;
using Checkout.PaymentGateway.Domain.Events;

namespace Checkout.PaymentGateway.ComponentTests
{
    public static class TestPayment
    {
        public static Payment ValidPayment(Guid id)
        {
            return new Payment(new PaymentCreatedEvent
            {
                Id = id,
                AcquiringBankId = Guid.NewGuid(),
                Amount = 100,
                Currency = Currency.GBP,
                CardNumber = "5555555555554444",
                Cvv = "343",
                ExpiryMonth = 12,
                ExpiryYear = 2022,
                Status = PaymentStatus.Authorized
            });
        }
    }

    public static class TestCreatePaymentRequests
    {
        public static PaymentRequest Configure(Action<PaymentRequest> act)
        {
            var request = ValidRequest;
            act(request);
            return request;
        }

        public static PaymentRequest ValidRequest => new PaymentRequest
        {
            Id = Guid.NewGuid(),
            Amount = 100,
            Currency = Currency.GBP,
            CardNumber = "5555555555554444",
            Cvv = "343",
            ExpiryMonth = 12,
            ExpiryYear = 2022
        };
    }
}