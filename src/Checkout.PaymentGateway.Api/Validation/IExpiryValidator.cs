namespace Checkout.PaymentGateway.Api.Validation
{
    public interface IExpiryValidator
    {
        public bool Validate(int month, int year);
    }
}