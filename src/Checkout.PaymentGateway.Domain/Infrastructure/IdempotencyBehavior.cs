using Checkout.PaymentGateway.Domain.Store;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Checkout.PaymentGateway.Domain.Infrastructure
{
    /// <summary>
    /// Caches a response against a corresponding idempotency key, if the key is already in use, throws an IdempotencyException
    /// </summary>
    public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TResponse : class
    {
        private readonly IIdempotencyStore<TResponse> _store;

        public IdempotencyBehavior(IIdempotencyStore<TResponse> store)
        {
            _store = store;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is IIdempotentRequest<TResponse> idempotencyRequest)
            {
                var addedSuccessfully = _store.Add(idempotencyRequest.IdempotencyKey, default);

                // if we can add our idempotency key to the store then we can proceed with running the command.
                if (addedSuccessfully)
                {
                    try
                    {
                        var response = await next();

                        _store.Update(idempotencyRequest.IdempotencyKey, response); 

                        return response;
                    }
                    catch (Exception)
                    {
                        _store.Remove(idempotencyRequest.IdempotencyKey);
                        throw;
                    }
                }
                else
                {
                    // If the key has already been added we can try to return the response from the original operation.
                    if (_store.Get(idempotencyRequest.IdempotencyKey, out var result) && result != default(TResponse))
                        return result;

                    // If there is no existing response then there may be multiple requests being processed concurrently
                    throw new IdempotencyViolationException();
                }
            }

            return await next();
        }
    }
}
