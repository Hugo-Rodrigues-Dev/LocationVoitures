using LocationVoitures.ApiService.Features.Locations.Requests;
using LocationVoitures.ApiService.Features.Locations.Validators;

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
            DateFin = new DateOnly(2026, 5, 12),
            NumeroCarte = "4242424242424242",
            MoisExpiration = 12,
            AnneeExpiration = 2030
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
            DateFin = new DateOnly(2026, 5, 12),
            NumeroCarte = "4242424242424242",
            MoisExpiration = 12,
            AnneeExpiration = 2030
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
            DateFin = new DateOnly(2026, 5, 10),
            NumeroCarte = "4242424242424242",
            MoisExpiration = 12,
            AnneeExpiration = 2030
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
            DateFin = new DateOnly(2026, 5, 12),
            NumeroCarte = "4242424242424242",
            MoisExpiration = 12,
            AnneeExpiration = 2030
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(ReserverRequest.Immatriculation)), Is.True);
    }

    [Test]
    public void ShouldRejectMissingCardNumber()
    {
        var request = new ReserverRequest
        {
            Immatriculation = "AA-123-BB",
            LoueurId = 1,
            DateDebut = new DateOnly(2026, 5, 10),
            DateFin = new DateOnly(2026, 5, 12),
            NumeroCarte = string.Empty,
            MoisExpiration = 12,
            AnneeExpiration = 2030
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(ReserverRequest.NumeroCarte)), Is.True);
    }

    [Test]
    public void ShouldRejectInvalidExpirationMonth()
    {
        var request = new ReserverRequest
        {
            Immatriculation = "AA-123-BB",
            LoueurId = 1,
            DateDebut = new DateOnly(2026, 5, 10),
            DateFin = new DateOnly(2026, 5, 12),
            NumeroCarte = "4242424242424242",
            MoisExpiration = 13,
            AnneeExpiration = 2030
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(ReserverRequest.MoisExpiration)), Is.True);
    }
}
