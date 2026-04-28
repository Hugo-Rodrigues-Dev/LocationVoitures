using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LocationVoitures.ApiService.Features.Voitures;

public class CreateVoitureRequest
{
    [Required]
    [RegularExpression("^[A-Z]{2}-\\d{3}-[A-Z]{2}$")]
    [Description("Immatriculation de la voiture au format AA-123-BB.")]
    public string Immatriculation { get; set; } = string.Empty;

    [Required]
    [Description("Marque commerciale de la voiture.")]
    public string Marque { get; set; } = string.Empty;

    [Required]
    [Description("Modele commercial de la voiture.")]
    public string Modele { get; set; } = string.Empty;

    [Description("Categorie de la voiture, par exemple Citadine, SUV ou Berline.")]
    public string? Categorie { get; set; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    [Description("Prix de location journalier. Doit etre strictement positif.")]
    public decimal PrixLocationParJour { get; set; }
}
