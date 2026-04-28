using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LocationVoitures.ApiService.Domain;

[Table("loueur")]
[Index(nameof(Mobile), IsUnique = true)]
public class Loueur
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("nom")]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("prenom")]
    public string Prenom { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("mobile")]
    public string Mobile { get; set; } = string.Empty;

    [MaxLength(255)]
    [Column("email")]
    public string? Email { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("pays")]
    public string Pays { get; set; } = "France";

    [Column("est_blacklist")]
    public bool EstBlacklist { get; set; }

    public ICollection<Location> Locations { get; set; } = [];
}
