using System;
using System.ComponentModel.DataAnnotations;

namespace Checkout.PaymentGateway.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class EnumIsDefinedAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            var enumValue = value as Enum;

            if (!Enum.IsDefined(enumValue.GetType(), enumValue))
                return new ValidationResult($"Unsupported value in field'{validationContext.DisplayName}'");

            return ValidationResult.Success;
        }
    }
}
