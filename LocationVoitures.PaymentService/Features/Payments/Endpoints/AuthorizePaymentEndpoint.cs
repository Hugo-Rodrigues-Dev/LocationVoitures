using FluentValidation;
using LocationVoitures.PaymentService.Features.Payments.Requests;
using LocationVoitures.PaymentService.Features.Payments.Responses;
using LocationVoitures.PaymentService.Features.Payments.Services;

namespace LocationVoitures.PaymentService.Features.Payments.Endpoints;

public static class AuthorizePaymentEndpoint
{
    public static void MapAuthorizePayment(this IEndpointRouteBuilder app)
    {
        app.MapPost("/payments/authorize",
            async (PaymentAuthorizationRequest request,
                IValidator<PaymentAuthorizationRequest> validator,
                CardValidationService cardValidationService) =>
            {
                var validation = await validator.ValidateAsync(request);

                if (!validation.IsValid)
                {
                    return Results.ValidationProblem(validation.ToDictionary());
                }

                var normalizedNumber = cardValidationService.NormalizeCardNumber(request.CardNumber);

                if (!cardValidationService.IsValidLuhn(normalizedNumber))
                {
                    return Results.Ok(new PaymentAuthorizationResponse
                    {
                        IsAuthorized = false,
                        Code = "card_number_invalid",
                        Message = "Le numero de carte bancaire est invalide."
                    });
                }

                if (cardValidationService.IsExpired(request.ExpirationMonth, request.ExpirationYear))
                {
                    return Results.Ok(new PaymentAuthorizationResponse
                    {
                        IsAuthorized = false,
                        Code = "card_expired",
                        Message = "La carte bancaire est expiree."
                    });
                }

                return Results.Ok(new PaymentAuthorizationResponse
                {
                    IsAuthorized = true,
                    Code = "authorized",
                    Message = "Paiement autorise."
                });
            })
            .WithName("AuthorizePayment")
            .WithTags("Paiements")
            .WithSummary("Autorise un paiement fictif")
            .WithDescription("Valide le numero de carte via Luhn, controle la date d'expiration et renvoie une autorisation de paiement fictive.")
            .Produces<PaymentAuthorizationResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
