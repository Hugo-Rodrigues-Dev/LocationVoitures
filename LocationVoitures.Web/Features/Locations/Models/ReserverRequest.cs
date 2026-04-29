using System.ComponentModel.DataAnnotations;

namespace LocationVoitures.Web.Features.Locations.Models;

public class ReserverRequest
{
    [Required(ErrorMessage = "L'immatriculation est obligatoire.")]
    [RegularExpression("^[A-Z]{2}-\\d{3}-[A-Z]{2}$", ErrorMessage = "Le format attendu est AA-123-BB.")]
    public string Immatriculation { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Le loueur est obligatoire.")]
    public int LoueurId { get; set; }

    [Required(ErrorMessage = "La date de debut est obligatoire.")]
    public DateOnly DateDebut { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "La date de fin est obligatoire.")]
    public DateOnly DateFin { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
}
