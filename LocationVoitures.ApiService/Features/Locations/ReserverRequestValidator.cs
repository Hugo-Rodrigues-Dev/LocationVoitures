using FluentValidation;

namespace LocationVoitures.ApiService.Features.Locations;

public class ReserverRequestValidator : AbstractValidator<ReserverRequest>
{
    private const string ImmatriculationPattern = "^[A-Z]{2}-\\d{3}-[A-Z]{2}$";

    public ReserverRequestValidator()
    {
        RuleFor(request => request.Immatriculation)
            .NotEmpty().WithMessage("L'immatriculation est obligatoire.")
            .Matches(ImmatriculationPattern).WithMessage("L'immatriculation doit respecter le format AA-123-BB.");

        RuleFor(request => request.LoueurId)
            .GreaterThan(0).WithMessage("Le LoueurId doit etre strictement positif.");

        RuleFor(request => request.DateDebut)
            .NotEqual(default(DateOnly)).WithMessage("La date de debut est obligatoire.");

        RuleFor(request => request.DateFin)
            .NotEqual(default(DateOnly)).WithMessage("La date de fin est obligatoire.");

        RuleFor(request => request.DateFin)
            .GreaterThanOrEqualTo(request => request.DateDebut)
            .WithMessage("La date de fin doit etre superieure ou egale a la date de debut.");
    }
}
