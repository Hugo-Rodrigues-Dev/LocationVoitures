using System.Net.Http.Json;
using LocationVoitures.PaymentService;
using LocationVoitures.PaymentService.Features.Payments.Requests;
using LocationVoitures.PaymentService.Features.Payments.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LocationVoitures.Tests.Features.Paiements.Integration;

public class AuthorizePaymentEndpointTests
{
    private WebApplicationFactory<PaymentServiceAssemblyMarker> _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new WebApplicationFactory<PaymentServiceAssemblyMarker>();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task PostAuthorize_ShouldReturnAuthorized_ForValidCard()
    {
        var request = new PaymentAuthorizationRequest
        {
            CardNumber = "4242 4242 4242 4242",
            ExpirationMonth = 12,
            ExpirationYear = DateTime.UtcNow.Year + 2,
            Amount = 180m,
            Currency = "EUR"
        };

        using var response = await _client.PostAsJsonAsync("/payments/authorize", request);
        var payload = await response.Content.ReadFromJsonAsync<PaymentAuthorizationResponse>();

        Assert.That(response.IsSuccessStatusCode, Is.True);
        Assert.That(payload, Is.Not.Null);
        Assert.That(payload!.IsAuthorized, Is.True);
        Assert.That(payload.Code, Is.EqualTo("authorized"));
    }

    [Test]
    public async Task PostAuthorize_ShouldReturnBusinessRefusal_ForInvalidLuhn()
    {
        var request = new PaymentAuthorizationRequest
        {
            CardNumber = "4242 4242 4242 4241",
            ExpirationMonth = 12,
            ExpirationYear = DateTime.UtcNow.Year + 2,
            Amount = 180m,
            Currency = "EUR"
        };

        using var response = await _client.PostAsJsonAsync("/payments/authorize", request);
        var payload = await response.Content.ReadFromJsonAsync<PaymentAuthorizationResponse>();

        Assert.That(response.IsSuccessStatusCode, Is.True);
        Assert.That(payload, Is.Not.Null);
        Assert.That(payload!.IsAuthorized, Is.False);
        Assert.That(payload.Code, Is.EqualTo("card_number_invalid"));
    }

    [Test]
    public async Task PostAuthorize_ShouldReturnBusinessRefusal_ForExpiredCard()
    {
        var now = DateTime.UtcNow;
        var expiredMonth = now.Month == 1 ? 12 : now.Month - 1;
        var expiredYear = now.Month == 1 ? now.Year - 1 : now.Year;

        var request = new PaymentAuthorizationRequest
        {
            CardNumber = "4242 4242 4242 4242",
            ExpirationMonth = expiredMonth,
            ExpirationYear = expiredYear,
            Amount = 180m,
            Currency = "EUR"
        };

        using var response = await _client.PostAsJsonAsync("/payments/authorize", request);
        var payload = await response.Content.ReadFromJsonAsync<PaymentAuthorizationResponse>();

        Assert.That(response.IsSuccessStatusCode, Is.True);
        Assert.That(payload, Is.Not.Null);
        Assert.That(payload!.IsAuthorized, Is.False);
        Assert.That(payload.Code, Is.EqualTo("card_expired"));
    }

    [Test]
    public async Task PostAuthorize_ShouldReturnBadRequest_ForMissingCurrency()
    {
        var request = new PaymentAuthorizationRequest
        {
            CardNumber = "4242 4242 4242 4242",
            ExpirationMonth = 12,
            ExpirationYear = DateTime.UtcNow.Year + 2,
            Amount = 180m,
            Currency = string.Empty
        };

        using var response = await _client.PostAsJsonAsync("/payments/authorize", request);
        var payload = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        Assert.That((int)response.StatusCode, Is.EqualTo(400));
        Assert.That(payload, Is.Not.Null);
        Assert.That(payload!.Errors.ContainsKey(nameof(PaymentAuthorizationRequest.Currency)), Is.True);
    }
}
