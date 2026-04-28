using LocationVoitures.ApiService.Features.Voitures.Requests;
using LocationVoitures.ApiService.Features.Voitures.Validators;

namespace LocationVoitures.Tests.Features.Voitures;

public class UpdatePrixVoitureRequestValidatorTests
{
    private readonly UpdatePrixVoitureRequestValidator _validator = new();

    [Test]
    public void ShouldAcceptPositivePrice()
    {
        var request = new UpdatePrixVoitureRequest
        {
            PrixLocationParJour = 70m
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ShouldRejectNonPositivePrice()
    {
        var request = new UpdatePrixVoitureRequest
        {
            PrixLocationParJour = 0m
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(error => error.PropertyName == nameof(UpdatePrixVoitureRequest.PrixLocationParJour)), Is.True);
    }
}
