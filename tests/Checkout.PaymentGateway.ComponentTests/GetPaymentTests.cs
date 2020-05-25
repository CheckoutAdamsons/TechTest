using Checkout.PaymentGateway.Api;
using Checkout.PaymentGateway.Domain.Store;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Api.V1;
using Checkout.PaymentGateway.Domain;
using Newtonsoft.Json;
using Xunit;

namespace Checkout.PaymentGateway.ComponentTests
{
    public class GetPaymentTests
    {
        private readonly Mock<IPaymentStore> _paymentStore = new Mock<IPaymentStore>();
        private readonly HttpClient _client;

        private readonly Guid _paymentId;
        private const string MerchantId = "GetPaymentAcceptanceTest";

        public GetPaymentTests()
        {
            var factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddTransient(provider => _paymentStore.Object);
                });
            });

            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(MerchantId);

            _paymentId = Guid.NewGuid();
        }

        [Fact]
        public async Task GivenAPaymentId_WhenTheUserIsNotAuthenticated_ThenReturn401()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await GetAsync(_paymentId);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GivenAPaymentId_WhenThePaymentIsNotFound_ThenReturn404()
        {
            var response = await GetAsync(_paymentId);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenAPaymentId_WhenAnotherMerchantHasCreatedATransactionWithTheSameId_ThenReturn404()
        {
            SetupPaymentStoreResponse(_paymentId);

            await GetAsync(_paymentId);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("DifferentId");

            var response = await GetAsync(_paymentId);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GivenAPaymentId_WhenThePaymentIdIsInvalid_ThenReturn400()
        {
            var response = await GetAsync(Guid.Empty);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenAPaymentId_WhenPaymentIsFound_ThenReturn200()
        {
            SetupPaymentStoreResponse(_paymentId);

            var response = await GetAsync(_paymentId);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GivenAPaymentId_WhenPaymentIsFound_ThenReturnMaskedPayment()
        {
            var payment = TestPayment.ValidPayment(_paymentId);
            SetupPaymentStoreResponse(_paymentId, payment);

            var response = await GetAsync(_paymentId);

            var paymentResponse = await ReadAsAsync<PaymentResponse>(response);

            paymentResponse.CardNumber.Should().NotBe(payment.CardNumber);
            paymentResponse.CardNumber.Should().Be(payment.MaskedCardNumber);
        }

        [Fact]
        public async Task GivenAPaymentId_WhenPaymentIsFound_ThenReturnPaymentStatus()
        {
            var payment = TestPayment.ValidPayment(_paymentId);
            SetupPaymentStoreResponse(_paymentId, payment);

            var response = await GetAsync(_paymentId);

            var paymentResponse = await ReadAsAsync<PaymentResponse>(response);

            paymentResponse.Status.Should().Be(payment.Status);
        }

        [Fact]
        public async Task GivenAPaymentId_WhenPaymentIsFound_ThenReturnPayment()
        {
            var payment = TestPayment.ValidPayment(_paymentId);
            SetupPaymentStoreResponse(_paymentId, payment);

            var response = await GetAsync(_paymentId);

            var paymentResponse = await ReadAsAsync<PaymentResponse>(response);

            paymentResponse.Id.Should().Be(_paymentId);
            paymentResponse.ExpiryMonth.Should().Be(payment.ExpiryMonth);
            paymentResponse.ExpiryYear.Should().Be(paymentResponse.ExpiryYear);
            paymentResponse.Currency.Should().Be(paymentResponse.Currency);
            paymentResponse.Amount.Should().Be(paymentResponse.Amount);
        }

        private void SetupPaymentStoreResponse(Guid paymentId, Payment payment = null)
        {
            _paymentStore
                .Setup(o => o.Get($"{MerchantId}{paymentId}"))
                .Returns(payment ?? TestPayment.ValidPayment(paymentId));
        }

        private async Task<T> ReadAsAsync<T>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        private Task<HttpResponseMessage> GetAsync(Guid id)
        {
            return _client.GetAsync($"/api/v1/payments/{id}");
        }
    }
}
