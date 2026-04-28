using LocationVoitures.ApiService.Features.Locations;

namespace LocationVoitures.Tests.Features.Locations;

public class ReserverRequestValidatorTests
{
    private readonly ReserverRequestValidator _validator = new();

    [Test]
    public void ShouldAcceptValidRequest()
    {
        var request = new ReserverRequest
        {
            Immatriculation = "AA-123-BB",
            LoueurId = 1,
            DateDebut = new DateOnly(2026, 5, 10),
            DateFin = new DateOnly(2026, 5, 12)
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ShouldRejectMissingLoueurId()
    {
        var request = new ReserverRequest
        {
            Immatriculation = "AA-123-BB",
            LoueurId = 0,
            DateDebut = new DateOnly(2026, 5, 10),
            DateFin = new DateOnly(2026, 5, 12)
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(ReserverRequest.LoueurId)), Is.True);
    }

    [Test]
    public void ShouldRejectDateFinBeforeDateDebut()
    {
        var request = new ReserverRequest
        {
            Immatriculation = "AA-123-BB",
            LoueurId = 1,
            DateDebut = new DateOnly(2026, 5, 12),
            DateFin = new DateOnly(2026, 5, 10)
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(ReserverRequest.DateFin)), Is.True);
    }

    [Test]
    public void ShouldRejectInvalidImmatriculation()
    {
        var request = new ReserverRequest
        {
            Immatriculation = "1234",
            LoueurId = 1,
            DateDebut = new DateOnly(2026, 5, 10),
            DateFin = new DateOnly(2026, 5, 12)
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(ReserverRequest.Immatriculation)), Is.True);
    }
}
