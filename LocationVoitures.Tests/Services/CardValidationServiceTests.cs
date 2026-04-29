using LocationVoitures.PaymentService.Features.Payments.Services;

namespace LocationVoitures.Tests.Services;

public class CardValidationServiceTests
{
    private CardValidationService _service = null!;

    [SetUp]
    public void Setup()
    {
        _service = new CardValidationService();
    }

    [Test]
    public void IsValidLuhn_ShouldReturnTrue_ForKnownValidCardNumber()
    {
        var result = _service.IsValidLuhn("4242 4242 4242 4242");

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsValidLuhn_ShouldReturnFalse_ForInvalidCardNumber()
    {
        var result = _service.IsValidLuhn("4242 4242 4242 4241");

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsExpired_ShouldReturnTrue_ForPastMonthInCurrentYear()
    {
        var now = DateTime.UtcNow;
        var expiredMonth = now.Month == 1 ? 12 : now.Month - 1;
        var expiredYear = now.Month == 1 ? now.Year - 1 : now.Year;

        var result = _service.IsExpired(expiredMonth, expiredYear);

        Assert.That(result, Is.True);
    }

    [Test]
    public void IsExpired_ShouldReturnFalse_ForFutureExpiration()
    {
        var result = _service.IsExpired(12, DateTime.UtcNow.Year + 2);

        Assert.That(result, Is.False);
    }
}
