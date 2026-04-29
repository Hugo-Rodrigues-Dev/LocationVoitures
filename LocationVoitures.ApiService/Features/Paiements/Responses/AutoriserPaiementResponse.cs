namespace LocationVoitures.ApiService.Features.Paiements.Responses;

public class AutoriserPaiementResponse
{
    public bool IsAuthorized { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
