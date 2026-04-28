using System.ComponentModel;

namespace LocationVoitures.ApiService.Features.Locations.Responses;

public class ReserverResponse
{
    [Description("Identifiant de la location nouvellement creee.")]
    public int IdLocation { get; init; }

    [Description("Montant total de la reservation sur la periode demandee.")]
    public decimal PrixTotal { get; init; }

    [Description("Message de confirmation renvoye par l'API.")]
    public string Message { get; init; } = string.Empty;
}
