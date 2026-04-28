namespace LocationVoitures.ApiService.Features.Locations;

public class ReserverRequest
{
    public string Immatriculation { get; set; } = string.Empty;
    public int LoueurId { get; set; }
    public DateOnly DateDebut { get; set; }
    public DateOnly DateFin { get; set; }
}
