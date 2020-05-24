using System;

namespace Checkout.PaymentGateway.Domain.Clients
{
    public enum AcquiringBankPaymentStatus
    {
        Authorized,
        Declined
    }

    public static class AcquiringBankPaymentStatusExtensions
    {
        public static PaymentStatus ToPaymentStatus(this AcquiringBankPaymentStatus value)
        {
            return value switch
            {
                AcquiringBankPaymentStatus.Authorized => PaymentStatus.Authorized,
                AcquiringBankPaymentStatus.Declined => PaymentStatus.Declined,
                _ => throw new ArgumentException($"Unknown or unsupported enum value: '{value}'")
            };
        }
    }
}
