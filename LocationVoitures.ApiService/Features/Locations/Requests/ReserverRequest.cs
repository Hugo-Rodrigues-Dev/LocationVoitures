using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LocationVoitures.ApiService.Features.Locations.Requests;

public class ReserverRequest
{
    [Required]
    [RegularExpression("^[A-Z]{2}-\\d{3}-[A-Z]{2}$")]
    [Description("Immatriculation de la voiture a reserver.")]
    public string Immatriculation { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    [Description("Identifiant du loueur qui reserve la voiture.")]
    public int LoueurId { get; set; }

    [DataType(DataType.Date)]
    [Description("Date de debut de location, incluse.")]
    public DateOnly DateDebut { get; set; }

    [DataType(DataType.Date)]
    [Description("Date de fin de location, incluse.")]
    public DateOnly DateFin { get; set; }

    [Required]
    [Description("Numero de carte bancaire utilise pour le paiement.")]
    public string NumeroCarte { get; set; } = string.Empty;

    [Range(1, 12)]
    [Description("Mois d'expiration de la carte bancaire.")]
    public int MoisExpiration { get; set; }

    [Range(2000, 9999)]
    [Description("Annee d'expiration de la carte bancaire.")]
    public int AnneeExpiration { get; set; }
}
