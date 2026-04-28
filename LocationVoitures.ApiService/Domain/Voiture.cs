using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Domain;

[Table("voiture")]
[Index(nameof(Immatriculation), IsUnique = true)]
public class Voiture
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("immatriculation")]
    public string Immatriculation { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("marque")]
    public string Marque { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("modele")]
    public string Modele { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("categorie")]
    public string? Categorie { get; set; }

    [Column("prix_location_par_jour", TypeName = "numeric(10,2)")]
    public decimal PrixLocationParJour { get; set; }

    public ICollection<Location> Locations { get; set; } = [];
}
