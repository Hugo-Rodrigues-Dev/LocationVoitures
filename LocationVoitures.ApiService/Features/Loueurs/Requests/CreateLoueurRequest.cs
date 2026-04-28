using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LocationVoitures.ApiService.Features.Loueurs.Requests;

public class CreateLoueurRequest
{
    [Required]
    [Description("Nom de famille du loueur.")]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [Description("Prenom du loueur.")]
    public string Prenom { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^\\d{10}$")]
    [Description("Numero de mobile unique sur 10 chiffres.")]
    public string Mobile { get; set; } = string.Empty;

    [EmailAddress]
    [Description("Adresse email du loueur. Ce champ est optionnel.")]
    public string? Email { get; set; }

    [Required]
    [Description("Pays de rattachement du loueur.")]
    public string Pays { get; set; } = "France";
}
