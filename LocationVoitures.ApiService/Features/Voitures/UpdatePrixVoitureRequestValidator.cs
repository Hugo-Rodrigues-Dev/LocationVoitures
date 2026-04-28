using FluentValidation;

namespace LocationVoitures.ApiService.Features.Voitures;

public class UpdatePrixVoitureRequestValidator : AbstractValidator<UpdatePrixVoitureRequest>
{
    public UpdatePrixVoitureRequestValidator()
    {
        RuleFor(request => request.PrixLocationParJour)
            .GreaterThan(0).WithMessage("Le prix doit etre strictement positif.");
    }
}
