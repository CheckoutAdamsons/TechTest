using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api;
using Checkout.PaymentGateway.Domain.Clients;
using Checkout.PaymentGateway.Domain.Commands;
using Checkout.PaymentGateway.Domain.Store;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Checkout.PaymentGateway.ComponentTests
{
    public class CreatePaymentIdempotencyTests
    {
        private readonly Mock<IAcquiringBankClient> _bankClient = new Mock<IAcquiringBankClient>();
        private readonly Mock<IPaymentStore> _paymentStore = new Mock<IPaymentStore>();
        private readonly Mock<IIdempotencyStore<CommandResponse>> _idempotencyStore = new Mock<IIdempotencyStore<CommandResponse>>();

        private readonly HttpClient _client;

        public CreatePaymentIdempotencyTests()
        {
            var factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTransient(provider => _bankClient.Object);
                    services.AddTransient(provider => _paymentStore.Object);
                    services.AddTransient(provider => _idempotencyStore.Object);
                });
            });

            _client = factory.CreateClient();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("CreatePaymentAcceptanceTests");

            _bankClient
                .Setup(o => o.PostAsync(It.IsAny<AcquiringBankPaymentRequest>()))
                .ReturnsAsync(new AcquiringBankPaymentResponse
                {
                    Id = Guid.NewGuid(),
                    Status = AcquiringBankPaymentStatus.Authorized
                });
        }

        [Fact]
        public async Task GivenConcurrentPaymentRequests_WhenMultipleRequestsAreInFlight_Return429()
        {
            // Here we simulate the race condition when another request has already added the same idempotency key to the store but no response has yet been cached.
            _idempotencyStore.Setup(o => o.Add(It.IsAny<string>(), It.IsAny<CommandResponse>())).Returns(false);
            _idempotencyStore.Setup(o => o.Get(It.IsAny<string>(), out It.Ref<CommandResponse>.IsAny)).Returns(false);

            var response = await _client.PostAsync("/api/v1/payments", new StringContent(JsonConvert.SerializeObject(TestCreatePaymentRequests.ValidRequest), Encoding.UTF8, MediaTypeNames.Application.Json));

            response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }
    }
}