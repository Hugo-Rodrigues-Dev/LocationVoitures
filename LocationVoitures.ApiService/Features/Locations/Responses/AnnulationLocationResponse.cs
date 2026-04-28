using System.ComponentModel;

namespace LocationVoitures.ApiService.Features.Locations.Responses;

public class AnnulationLocationResponse
{
    [Description("Identifiant de la location modifiee.")]
    public int Id { get; init; }

    [Description("Vaut true lorsque la location est annulee.")]
    public bool Annule { get; init; }
}
