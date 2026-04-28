using FluentValidation;

namespace LocationVoitures.ApiService.Features.Loueurs;

public class CreateLoueurRequestValidator : AbstractValidator<CreateLoueurRequest>
{
    public CreateLoueurRequestValidator()
    {
        RuleFor(request => request.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire.")
            .MaximumLength(100).WithMessage("Le nom ne doit pas depasser 100 caracteres.");

        RuleFor(request => request.Prenom)
            .NotEmpty().WithMessage("Le prenom est obligatoire.")
            .MaximumLength(100).WithMessage("Le prenom ne doit pas depasser 100 caracteres.");

        RuleFor(request => request.Mobile)
            .NotEmpty().WithMessage("Le mobile est obligatoire.")
            .Matches("^\\d{10}$").WithMessage("Le mobile doit contenir exactement 10 chiffres.");

        RuleFor(request => request.Email)
            .EmailAddress().When(request => !string.IsNullOrWhiteSpace(request.Email))
            .WithMessage("L'email doit etre valide.");

        RuleFor(request => request.Pays)
            .NotEmpty().WithMessage("Le pays est obligatoire.")
            .MaximumLength(100).WithMessage("Le pays ne doit pas depasser 100 caracteres.");
    }
}
