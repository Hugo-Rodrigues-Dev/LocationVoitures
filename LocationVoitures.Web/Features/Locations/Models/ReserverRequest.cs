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

    [Required(ErrorMessage = "Le numero de carte est obligatoire.")]
    public string NumeroCarte { get; set; } = string.Empty;

    [Range(1, 12, ErrorMessage = "Le mois d'expiration doit etre compris entre 1 et 12.")]
    public int MoisExpiration { get; set; } = DateTime.Today.Month;

    [Range(2000, 9999, ErrorMessage = "L'annee d'expiration doit etre valide.")]
    public int AnneeExpiration { get; set; } = DateTime.Today.Year + 1;
}
