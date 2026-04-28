namespace LocationVoitures.ApiService.Features.Voitures;

public class CreateVoitureRequest
{
    public string Immatriculation { get; set; } = string.Empty;
    public string Marque { get; set; } = string.Empty;
    public string Modele { get; set; } = string.Empty;
    public string? Categorie { get; set; }
    public decimal PrixLocationParJour { get; set; }
}
