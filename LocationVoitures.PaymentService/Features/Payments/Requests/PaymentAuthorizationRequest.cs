using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LocationVoitures.PaymentService.Features.Payments.Requests;

public class PaymentAuthorizationRequest
{
    [Required]
    [Description("Numero de carte bancaire a verifier.")]
    public string CardNumber { get; init; } = string.Empty;

    [Range(1, 12)]
    [Description("Mois d'expiration de la carte.")]
    public int ExpirationMonth { get; init; }

    [Range(2000, 9999)]
    [Description("Annee d'expiration de la carte.")]
    public int ExpirationYear { get; init; }

    [Range(typeof(decimal), "0.01", "999999.99")]
    [Description("Montant a autoriser.")]
    public decimal Amount { get; init; }

    [Required]
    [Description("Devise du paiement.")]
    public string Currency { get; init; } = "EUR";
}
