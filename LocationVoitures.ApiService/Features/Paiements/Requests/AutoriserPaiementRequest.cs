namespace LocationVoitures.ApiService.Features.Paiements.Requests;

public class AutoriserPaiementRequest
{
    public string CardNumber { get; init; } = string.Empty;

    public int ExpirationMonth { get; init; }

    public int ExpirationYear { get; init; }

    public decimal Amount { get; init; }

    public string Currency { get; init; } = "EUR";
}
