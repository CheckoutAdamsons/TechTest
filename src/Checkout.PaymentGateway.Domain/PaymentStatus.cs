namespace Checkout.PaymentGateway.Domain
{
    public enum PaymentStatus
    {
        None,
        Authorized, 
        Reversed,
        Declined
    }
}
