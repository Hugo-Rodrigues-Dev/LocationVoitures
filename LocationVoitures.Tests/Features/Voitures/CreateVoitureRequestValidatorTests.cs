using LocationVoitures.ApiService.Features.Voitures.Requests;
using LocationVoitures.ApiService.Features.Voitures.Validators;

namespace LocationVoitures.Tests.Features.Voitures;

public class CreateVoitureRequestValidatorTests
{
    private readonly CreateVoitureRequestValidator _validator = new();

    [Test]
    public void ShouldAcceptValidRequest()
    {
        var request = new CreateVoitureRequest
        {
            Immatriculation = "AA-123-BB",
            Marque = "Renault",
            Modele = "Clio",
            Categorie = "Citadine",
            PrixLocationParJour = 55m
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ShouldRejectInvalidImmatriculation()
    {
        var request = new CreateVoitureRequest
        {
            Immatriculation = "INVALID",
            Marque = "Renault",
            Modele = "Clio",
            PrixLocationParJour = 55m
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(CreateVoitureRequest.Immatriculation)), Is.True);
    }

    [Test]
    public void ShouldRejectMissingMarque()
    {
        var request = new CreateVoitureRequest
        {
            Immatriculation = "AA-123-BB",
            Marque = string.Empty,
            Modele = "Clio",
            PrixLocationParJour = 55m
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(CreateVoitureRequest.Marque)), Is.True);
    }

    [Test]
    public void ShouldRejectNonPositivePrice()
    {
        var request = new CreateVoitureRequest
        {
            Immatriculation = "AA-123-BB",
            Marque = "Renault",
            Modele = "Clio",
            PrixLocationParJour = 0m
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(CreateVoitureRequest.PrixLocationParJour)), Is.True);
    }
}
