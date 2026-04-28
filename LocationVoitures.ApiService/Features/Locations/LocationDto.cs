using System.ComponentModel;

namespace LocationVoitures.ApiService.Features.Locations;

public class LocationDto
{
    [Description("Identifiant unique de la location.")]
    public int Id { get; init; }

    [Description("Immatriculation de la voiture reservee.")]
    public string Immatriculation { get; init; } = string.Empty;

    [Description("Identifiant du loueur qui a effectue la reservation.")]
    public int LoueurId { get; init; }

    [Description("Date de debut de location, incluse.")]
    public DateOnly DateDebut { get; init; }

    [Description("Date de fin de location, incluse.")]
    public DateOnly DateFin { get; init; }

    [Description("Indique si la location a ete annulee.")]
    public bool Annule { get; init; }
}
