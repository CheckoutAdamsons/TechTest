using System;
using System.ComponentModel.DataAnnotations;

namespace Checkout.PaymentGateway.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class NotEmptyGuidAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            if ((value is Guid) && (Guid)value == Guid.Empty)
                return new ValidationResult($"${validationContext.DisplayName} must not be empty.");

            return ValidationResult.Success;
        }
    }
}