using System;

namespace Checkout.PaymentGateway.Domain.Infrastructure
{
    public class IdempotencyViolationException : Exception
    {
        public IdempotencyViolationException() { }

        public IdempotencyViolationException(string message) : base(message)  {  }

        public IdempotencyViolationException(string message, Exception inner) : base(message, inner) { }
    }
}