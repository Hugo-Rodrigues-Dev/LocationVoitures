using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LocationVoitures.ApiService.Features.Locations;

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
}
