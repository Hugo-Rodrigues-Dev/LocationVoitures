namespace LocationVoitures.Web.Features.Voitures.Models;

public class VoitureDto
{
    public string Immatriculation { get; set; } = string.Empty;

    public string Marque { get; set; } = string.Empty;

    public string Modele { get; set; } = string.Empty;

    public string Categorie { get; set; } = string.Empty;

    public decimal PrixParJour { get; set; }

    public bool EstDisponible { get; set; }
}
