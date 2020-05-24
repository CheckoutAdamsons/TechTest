using Checkout.PaymentGateway.Domain.Infrastructure;
using Checkout.PaymentGateway.Domain.Store;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.PaymentGateway.Domain
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddSingleton<IPaymentStore, PaymentStore>();
            services.AddScoped(typeof(IIdempotencyStore<>), typeof(IdempotencyStore<>));
            return services;
        }
    }
}
