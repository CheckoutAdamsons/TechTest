using System;
using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Domain.Clients;
using Checkout.PaymentGateway.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.Domain.Commands
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, CommandResponse>
    {
        private readonly IAcquiringBankClient _acquiringBankClient;
        private readonly IMediator _mediator;
        private readonly ILogger<CreatePaymentCommandHandler> _logger;

        public CreatePaymentCommandHandler(
            IAcquiringBankClient acquiringBankClient,
            IMediator mediator,
            ILogger<CreatePaymentCommandHandler> logger)
        {
            _acquiringBankClient = acquiringBankClient ?? throw new ArgumentNullException(nameof(acquiringBankClient));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<CommandResponse> Handle(CreatePaymentCommand command, CancellationToken cts)
        {
            AcquiringBankPaymentResponse result;

            try
            {
                result = await _acquiringBankClient.PostAsync(new AcquiringBankPaymentRequest
                {
                    Amount = command.Amount,
                    CardNumber = command.CardNumber,
                    Currency = command.Currency.ToString(),
                    ExpiryMonth = command.ExpiryMonth,
                    ExpiryYear = command.ExpiryYear,
                    Cvv = command.Cvv
                });
            }
            catch (Exception ex) // here we could raise a PaymentCreatedFailedEvent and in some cases, like a client timeout, have a handler which voids the payment.
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

            await _mediator.Publish(new PaymentCreatedEvent
            {
                Id = command.Id,
                AcquiringBankId = result.Id,
                Amount = command.Amount,
                CardNumber = command.CardNumber,
                ExpiryMonth = command.ExpiryMonth,
                ExpiryYear = command.ExpiryYear,
                Cvv = command.Cvv,
                Currency = command.Currency,
                Status = result.Status.ToPaymentStatus()
            });

            return new CommandResponse
            {
                Id = command.Id,
                Success = true
            };
        }
    }
}
