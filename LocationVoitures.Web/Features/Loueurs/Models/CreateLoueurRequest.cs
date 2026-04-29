using System.ComponentModel.DataAnnotations;

namespace LocationVoitures.Web.Features.Loueurs.Models;

public class CreateLoueurRequest
{
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string Nom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le prenom est obligatoire.")]
    public string Prenom { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le mobile est obligatoire.")]
    [RegularExpression("^\\d{10}$", ErrorMessage = "Le mobile doit contenir exactement 10 chiffres.")]
    public string Mobile { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "L'email doit etre valide.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Le pays est obligatoire.")]
    public string Pays { get; set; } = "France";
}
