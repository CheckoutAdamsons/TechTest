using System;

namespace Checkout.PaymentGateway.Domain.Commands
{
    public class CommandResponse
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
    }
}
