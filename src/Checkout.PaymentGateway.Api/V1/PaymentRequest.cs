using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Checkout.PaymentGateway.Api.Attributes;
using Checkout.PaymentGateway.Api.Validation;
using Checkout.PaymentGateway.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.PaymentGateway.Api.V1
{
    public class PaymentRequest : IValidatableObject
    {
        // TODO - move the idempotency behavior on this to a separate http header e.g. x-idempotency-key
        /// <summary>
        /// The payment Id, also used as a key for idempotency
        /// </summary>
        [Required, NotEmptyGuid]
        public Guid Id { get; set; }

        // I think the max transaction value here could be configurable per merchant, if not dynamic.
        /// <summary>
        /// The value of the transaction in the selected currency
        /// </summary>
        /// <example>100</example>
        [Required, Range(0, int.MaxValue, ErrorMessage = "Amount must be 0 or greater.")]
        public int? Amount { get; set; }

        /// <summary>
        /// The card number 
        /// </summary>
        /// <example>5555555555554444</example>
        [Required, CreditCard]
        public string CardNumber { get; set; }

        /// <summary>
        /// The expiry month of the card
        /// </summary>
        /// <example>12</example>
        [Required, Range(1, 12)]
        public int? ExpiryMonth { get; set; }

        /// <summary>
        /// The expiry year of the card
        /// </summary>
        /// <example>2022</example>
        [Required]
        public int? ExpiryYear { get; set; }

        /// <summary>
        /// The card verification value/code. 3 digits, except for Amex (4 digits)
        /// </summary>
        /// <example>999</example>
        [Required, StringLength(4, MinimumLength = 3, ErrorMessage = "Cvv must be between 3 and 4 characters")]
        public string Cvv { get; set; }

        /// <summary>
        /// The iso 4217 alpha-3 currency code
        /// </summary>
        /// <example>GBP</example>
        [Required, EnumIsDefined]
        public Currency Currency { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationService = validationContext.GetService<IExpiryValidator>();

            if (ExpiryMonth.HasValue && ExpiryYear.HasValue)
            {
                if (validationService.Validate(ExpiryMonth.Value, ExpiryYear.Value))
                {
                    return new[] { ValidationResult.Success };
                }

                return new[] { new ValidationResult("Card expiry is in the past."),  };
            }

            return new[] { ValidationResult.Success };
        }
    }
}
