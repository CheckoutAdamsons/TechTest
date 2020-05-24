using System.ComponentModel.DataAnnotations;

namespace Checkout.PaymentGateway.AcquiringBankSimulator
{
    public class PaymentRequest
    {
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
        [Required, StringLength(4, MinimumLength = 3)]
        public string Cvv { get; set; }

        /// <summary>
        /// The iso 4217 alpha-3 currency code
        /// </summary>
        /// <example>GBP</example>
        [Required]
        public string Currency { get; set; }
    }
}