using LocationVoitures.PaymentService.Features.Payments.Requests;
using LocationVoitures.PaymentService.Features.Payments.Validators;

namespace LocationVoitures.Tests.Features.Paiements.Validators;

public class PaymentAuthorizationRequestValidatorTests
{
    private readonly PaymentAuthorizationRequestValidator _validator = new();

    [Test]
    public void ShouldAcceptValidRequest()
    {
        var request = new PaymentAuthorizationRequest
        {
            CardNumber = "4242 4242 4242 4242",
            ExpirationMonth = 12,
            ExpirationYear = DateTime.UtcNow.Year + 2,
            Amount = 160m,
            Currency = "EUR"
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ShouldRejectMissingCurrency()
    {
        var request = new PaymentAuthorizationRequest
        {
            CardNumber = "4242 4242 4242 4242",
            ExpirationMonth = 12,
            ExpirationYear = DateTime.UtcNow.Year + 2,
            Amount = 160m,
            Currency = string.Empty
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(PaymentAuthorizationRequest.Currency)), Is.True);
    }

    [Test]
    public void ShouldRejectAmountAtZero()
    {
        var request = new PaymentAuthorizationRequest
        {
            CardNumber = "4242 4242 4242 4242",
            ExpirationMonth = 12,
            ExpirationYear = DateTime.UtcNow.Year + 2,
            Amount = 0m,
            Currency = "EUR"
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(PaymentAuthorizationRequest.Amount)), Is.True);
    }
}
