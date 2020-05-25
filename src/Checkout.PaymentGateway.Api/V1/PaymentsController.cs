using Checkout.PaymentGateway.Api.Attributes;
using Checkout.PaymentGateway.Domain.Commands;
using Checkout.PaymentGateway.Domain.Infrastructure;
using Checkout.PaymentGateway.Domain.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Api.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContext;

        public PaymentsController(
            IMediator mediator,
            IHttpContextAccessor httpContext)
        {
            _mediator = mediator;
            _httpContext = httpContext;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Post([FromBody] PaymentRequest request)
        {
            var command = new CreatePaymentCommand(
                request.Id,
                _httpContext.HttpContext.User.Identity.Name,
                request.Amount.Value,
                request.CardNumber,
                request.ExpiryMonth.Value,
                request.ExpiryYear.Value,
                request.Cvv,
                request.Currency);
            try
            {
                // The mediator routes this command to the CreatePaymentCommandHandler.cs
                var result = await _mediator.Send(command, HttpContext.RequestAborted);

                if (result.Success)
                    return Created($"/api/v1/payments/{result.Id}", result);

                return UnprocessableEntity("Payment could not be processed.");
            }
            catch (IdempotencyViolationException) // TODO - Extract this to a middleware
            {
                return new StatusCodeResult(429);
            }
        }

        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([Required, NotEmptyGuid] Guid id)
        {
            var merchantId = _httpContext.HttpContext.User.Identity.Name;
            var query = new GetPaymentQuery(id, merchantId);

            // The mediator routes this query to the GetPaymentQueryHandler.cs
            var payment = await _mediator.Send(query);

            if (payment == null)
                return NotFound();

            return Ok(new PaymentResponse(payment));
        }
    }
}
