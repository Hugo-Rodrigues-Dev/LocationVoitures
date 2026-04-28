using LocationVoitures.ApiService.Features.Loueurs.Requests;
using LocationVoitures.ApiService.Features.Loueurs.Validators;

namespace LocationVoitures.Tests.Features.Loueurs;

public class CreateLoueurRequestValidatorTests
{
    private readonly CreateLoueurRequestValidator _validator = new();

    [Test]
    public void ShouldAcceptValidRequest()
    {
        var request = new CreateLoueurRequest
        {
            Nom = "Martin",
            Prenom = "Claire",
            Mobile = "0601020304",
            Email = "claire.martin@example.com",
            Pays = "France"
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ShouldRejectInvalidMobile()
    {
        var request = new CreateLoueurRequest
        {
            Nom = "Martin",
            Prenom = "Claire",
            Mobile = "06-01-02",
            Email = "claire.martin@example.com",
            Pays = "France"
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(CreateLoueurRequest.Mobile)), Is.True);
    }

    [Test]
    public void ShouldRejectInvalidEmail()
    {
        var request = new CreateLoueurRequest
        {
            Nom = "Martin",
            Prenom = "Claire",
            Mobile = "0601020304",
            Email = "not-an-email",
            Pays = "France"
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(CreateLoueurRequest.Email)), Is.True);
    }

    [Test]
    public void ShouldRejectMissingPays()
    {
        var request = new CreateLoueurRequest
        {
            Nom = "Martin",
            Prenom = "Claire",
            Mobile = "0601020304",
            Email = "claire.martin@example.com",
            Pays = string.Empty
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(CreateLoueurRequest.Pays)), Is.True);
    }
}
