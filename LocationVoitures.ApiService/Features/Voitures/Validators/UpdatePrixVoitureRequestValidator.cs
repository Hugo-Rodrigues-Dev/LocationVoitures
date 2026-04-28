using FluentValidation;
using LocationVoitures.ApiService.Features.Voitures.Requests;

namespace LocationVoitures.ApiService.Features.Voitures.Validators;

public class UpdatePrixVoitureRequestValidator : AbstractValidator<UpdatePrixVoitureRequest>
{
    public UpdatePrixVoitureRequestValidator()
    {
        RuleFor(request => request.PrixLocationParJour)
            .GreaterThan(0).WithMessage("Le prix doit etre strictement positif.");
    }
}
