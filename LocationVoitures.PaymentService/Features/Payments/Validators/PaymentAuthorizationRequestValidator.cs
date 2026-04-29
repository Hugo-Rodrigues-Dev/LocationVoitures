using FluentValidation;
using LocationVoitures.PaymentService.Features.Payments.Requests;

namespace LocationVoitures.PaymentService.Features.Payments.Validators;

public class PaymentAuthorizationRequestValidator : AbstractValidator<PaymentAuthorizationRequest>
{
    public PaymentAuthorizationRequestValidator()
    {
        RuleFor(request => request.CardNumber)
            .NotEmpty().WithMessage("Le numero de carte est obligatoire.");

        RuleFor(request => request.ExpirationMonth)
            .InclusiveBetween(1, 12).WithMessage("Le mois d'expiration doit etre compris entre 1 et 12.");

        RuleFor(request => request.ExpirationYear)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("L'annee d'expiration doit etre valide.");

        RuleFor(request => request.Amount)
            .GreaterThan(0m).WithMessage("Le montant doit etre strictement positif.");

        RuleFor(request => request.Currency)
            .NotEmpty().WithMessage("La devise est obligatoire.")
            .Length(3).WithMessage("La devise doit etre codee sur 3 caracteres.");
    }
}
