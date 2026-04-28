using System.ComponentModel;

namespace LocationVoitures.ApiService.Features.Voitures;

public class VoitureDto
{
    [Description("Immatriculation de la voiture au format AA-123-BB.")]
    public string Immatriculation { get; init; } = string.Empty;

    [Description("Marque commerciale de la voiture.")]
    public string Marque { get; init; } = string.Empty;

    [Description("Modele commercial de la voiture.")]
    public string Modele { get; init; } = string.Empty;

    [Description("Categorie de la voiture, par exemple Citadine, SUV ou Berline.")]
    public string Categorie { get; init; } = string.Empty;

    [Description("Prix de location pour une journee complete.")]
    public decimal PrixParJour { get; init; }

    [Description("Indique si la voiture est disponible a la date du jour.")]
    public bool EstDisponible { get; init; }
}
