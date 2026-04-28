using LocationVoitures.ApiService.Features.Locations;
using LocationVoitures.ApiService.Features.Loueurs;
using LocationVoitures.ApiService.Features.Voitures;

namespace LocationVoitures.Tests;

public class RequestValidatorsTests
{
    [Test]
    public void CreateVoitureRequestValidator_ShouldAcceptValidRequest()
    {
        var validator = new CreateVoitureRequestValidator();
        var request = new CreateVoitureRequest
        {
            Immatriculation = "AA-123-BB",
            Marque = "Renault",
            Modele = "Clio",
            Categorie = "Citadine",
            PrixLocationParJour = 55m
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void CreateVoitureRequestValidator_ShouldRejectInvalidImmatriculation()
    {
        var validator = new CreateVoitureRequestValidator();
        var request = new CreateVoitureRequest
        {
            Immatriculation = "INVALID",
            Marque = "Renault",
            Modele = "Clio",
            PrixLocationParJour = 55m
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(CreateVoitureRequest.Immatriculation)), Is.True);
    }

    [Test]
    public void ReserverRequestValidator_ShouldRejectMissingLoueurId()
    {
        var validator = new ReserverRequestValidator();
        var request = new ReserverRequest
        {
            Immatriculation = "AA-123-BB",
            LoueurId = 0,
            DateDebut = new DateOnly(2026, 5, 10),
            DateFin = new DateOnly(2026, 5, 12)
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(ReserverRequest.LoueurId)), Is.True);
    }

    [Test]
    public void ReserverRequestValidator_ShouldRejectDateFinBeforeDateDebut()
    {
        var validator = new ReserverRequestValidator();
        var request = new ReserverRequest
        {
            Immatriculation = "AA-123-BB",
            LoueurId = 1,
            DateDebut = new DateOnly(2026, 5, 12),
            DateFin = new DateOnly(2026, 5, 10)
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(ReserverRequest.DateFin)), Is.True);
    }

    [Test]
    public void UpdatePrixVoitureRequestValidator_ShouldRejectNonPositivePrice()
    {
        var validator = new UpdatePrixVoitureRequestValidator();
        var request = new UpdatePrixVoitureRequest
        {
            PrixLocationParJour = 0m
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(UpdatePrixVoitureRequest.PrixLocationParJour)), Is.True);
    }

    [Test]
    public void CreateLoueurRequestValidator_ShouldAcceptValidRequest()
    {
        var validator = new CreateLoueurRequestValidator();
        var request = new CreateLoueurRequest
        {
            Nom = "Martin",
            Prenom = "Claire",
            Mobile = "0601020304",
            Email = "claire.martin@example.com",
            Pays = "France"
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void CreateLoueurRequestValidator_ShouldRejectInvalidMobile()
    {
        var validator = new CreateLoueurRequestValidator();
        var request = new CreateLoueurRequest
        {
            Nom = "Martin",
            Prenom = "Claire",
            Mobile = "06-01-02",
            Email = "claire.martin@example.com",
            Pays = "France"
        };

        var result = validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(CreateLoueurRequest.Mobile)), Is.True);
    }
}
