using System.ComponentModel;

namespace LocationVoitures.PaymentService.Features.Payments.Responses;

public class PaymentAuthorizationResponse
{
    [Description("Indique si le paiement est autorise.")]
    public bool IsAuthorized { get; init; }

    [Description("Code technique simple renvoye par le service de paiement.")]
    public string Code { get; init; } = string.Empty;

    [Description("Message lisible a afficher a l'utilisateur ou dans les logs.")]
    public string Message { get; init; } = string.Empty;
}
