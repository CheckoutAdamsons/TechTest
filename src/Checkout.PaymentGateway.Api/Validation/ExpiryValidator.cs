using Microsoft.AspNetCore.Authentication;

namespace Checkout.PaymentGateway.Api.Validation
{
    public class ExpiryValidator : IExpiryValidator
    {
        private readonly ISystemClock _systemClock;

        public ExpiryValidator(ISystemClock systemClock)
        {
            _systemClock = systemClock;
        }

        public bool Validate(int expiryMonth, int expiryYear)
        {
            var currentTime = _systemClock.UtcNow;

            if (currentTime.Year > expiryYear)
                return false;

            if (currentTime.Year == expiryYear && currentTime.Month > expiryMonth)
                return false;

            return true;
        }
    }
}
