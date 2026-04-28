using FluentValidation;
using LocationVoitures.ApiService.Features.Voitures.Requests;

namespace LocationVoitures.ApiService.Features.Voitures.Validators;

public class CreateVoitureRequestValidator : AbstractValidator<CreateVoitureRequest>
{
    private const string ImmatriculationPattern = "^[A-Z]{2}-\\d{3}-[A-Z]{2}$";

    public CreateVoitureRequestValidator()
    {
        RuleFor(request => request.Immatriculation)
            .NotEmpty().WithMessage("L'immatriculation est obligatoire.")
            .Matches(ImmatriculationPattern).WithMessage("L'immatriculation doit respecter le format AA-123-BB.");

        RuleFor(request => request.Marque)
            .NotEmpty().WithMessage("La marque est obligatoire.");

        RuleFor(request => request.Modele)
            .NotEmpty().WithMessage("Le modele est obligatoire.");

        RuleFor(request => request.PrixLocationParJour)
            .GreaterThan(0).WithMessage("Le prix doit etre strictement positif.");
    }
}
