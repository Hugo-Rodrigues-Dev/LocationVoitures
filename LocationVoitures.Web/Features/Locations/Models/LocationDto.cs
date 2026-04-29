namespace LocationVoitures.Web.Features.Locations.Models;

public class LocationDto
{
    public int Id { get; set; }

    public string Immatriculation { get; set; } = string.Empty;

    public int LoueurId { get; set; }

    public string LoueurNomComplet { get; set; } = string.Empty;

    public DateOnly DateDebut { get; set; }

    public DateOnly DateFin { get; set; }

    public bool Annule { get; set; }

    public bool Paye { get; set; }
}
