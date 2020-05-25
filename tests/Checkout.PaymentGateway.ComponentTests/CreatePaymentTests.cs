using Checkout.PaymentGateway.Api;
using Checkout.PaymentGateway.Api.V1;
using Checkout.PaymentGateway.Domain.Clients;
using Checkout.PaymentGateway.Domain.Commands;
using Checkout.PaymentGateway.Domain.Events;
using Checkout.PaymentGateway.Domain.Store;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Checkout.PaymentGateway.ComponentTests
{
    public class CreatePaymentTests
    {
        private readonly Mock<IAcquiringBankClient> _bankClient = new Mock<IAcquiringBankClient>();
        private readonly Mock<IPaymentStore> _paymentStore = new Mock<IPaymentStore>();
        private readonly Mock<ISystemClock> _systemClock = new Mock<ISystemClock>();

        private readonly HttpClient _client;

        private const string PaymentV1Route = "/api/v1/payments/";

        public CreatePaymentTests()
        {
            var factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTransient(provider => _bankClient.Object);
                    services.AddTransient(provider => _paymentStore.Object);
                    services.AddTransient(provider => _systemClock.Object);
                });
            });

            _systemClock.Setup(o => o.UtcNow).Returns(DateTimeOffset.UtcNow);

            _client = factory.CreateClient();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("CreatePaymentAcceptanceTests");
        }

        [Fact]
        public async Task GivenAPaymentRequest_WhenTheUserIsNotAuthenticated_ThenReturn401()
        {
            SetupAcquiringBankResponse(AcquiringBankPaymentStatus.Authorized);

            _client.DefaultRequestHeaders.Authorization = null;

            var response = await PostAsync(TestCreatePaymentRequests.ValidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GivenAPaymentRequest_WhenTheRequestIsValid_ThenReturn201()
        {
            SetupAcquiringBankResponse(AcquiringBankPaymentStatus.Authorized);

            var response = await PostAsync(TestCreatePaymentRequests.ValidRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task GivenAPaymentRequest_WhenTheRequestIsValid_ThenReturnPaymentLocation()
        {
            SetupAcquiringBankResponse(AcquiringBankPaymentStatus.Authorized);

            var request = TestCreatePaymentRequests.ValidRequest;

            var response = await PostAsync(request);

            response.Headers.Location.OriginalString.Should().Be($"{PaymentV1Route}{request.Id}");
        }

        [Fact]
        public async Task GivenAPaymentRequest_WhenTheRequestIsValid_ThenReturnPaymentId()
        {
            SetupAcquiringBankResponse(AcquiringBankPaymentStatus.Authorized);

            var request = TestCreatePaymentRequests.ValidRequest;
            var response = await PostAsync(request);

            var responseContent = await ReadAsAsync<CommandResponse>(response);

            responseContent.Id.Should().Be(request.Id);
            responseContent.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GivenAPaymentRequest_WhenTheRequestIsResubmitted_ThenReturnCachedResponse()
        {
            SetupAcquiringBankResponse(AcquiringBankPaymentStatus.Authorized);

            var request = TestCreatePaymentRequests.ValidRequest;

            var responseA = await PostAsync(request);
            var responseB = await PostAsync(request);

            responseA.StatusCode.Should().Be(HttpStatusCode.Created);
            responseB.StatusCode.Should().Be(HttpStatusCode.Created);

            _paymentStore.Verify(o => o.Append(It.IsAny<IPaymentEvent>()), Times.Once);
        }

        [Fact]
        public async Task GivenAPaymentRequest_WhenTheCardIsExpired_ThenReturn400()
        {
            _systemClock.Setup(o => o.UtcNow).Returns(new DateTimeOffset(2200, 3, 1, 12, 0, 0, TimeSpan.Zero));

            var response = await PostAsync(TestCreatePaymentRequests.Configure(request =>
            {
                request.ExpiryYear = 2020;
                request.ExpiryMonth = 2;
            }));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [MemberData(nameof(InvalidCardPans))]
        public async Task GivenAPaymentRequest_WhenTheCardFailsLuhnCheck_ThenReturn400(string cardPan)
        {
            SetupAcquiringBankResponse(AcquiringBankPaymentStatus.Authorized);

            var response = await PostAsync(TestCreatePaymentRequests.Configure(request =>
            {
                request.CardNumber = cardPan;
            }));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public static IEnumerable<object[]> InvalidCardPans => new List<object[]>
        {
            new[] { "0" }, // weirdly this passes [CreditCard] validation
            new[] { "1" },
            new[] { "2" },
            new[] { "111111111111111111111111111" }, // This too (27 digits), looks like a bug
            new[] { "-1" },
            new[] { "Hello World" },
            new[] { "11111111111111" },
            new[] { "491509023274655" }
        };

        private void SetupAcquiringBankResponse(AcquiringBankPaymentStatus status)
        {
            _bankClient
                .Setup(o => o.PostAsync(It.IsAny<AcquiringBankPaymentRequest>()))
                .ReturnsAsync(new AcquiringBankPaymentResponse
                {
                    Id = Guid.NewGuid(),
                    Status = status
                });
        }

        private async Task<T> ReadAsAsync<T>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        private Task<HttpResponseMessage> PostAsync(PaymentRequest request)
        {
            return _client.PostAsync(PaymentV1Route, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, MediaTypeNames.Application.Json));
        }
    }
}
