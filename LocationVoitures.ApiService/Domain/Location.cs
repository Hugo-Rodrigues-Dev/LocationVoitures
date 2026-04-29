using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocationVoitures.ApiService.Domain;

[Table("location")]
public class Location
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("voiture_id")]
    public int VoitureId { get; set; }

    [ForeignKey(nameof(VoitureId))]
    public Voiture? Voiture { get; set; }

    [Column("loueur_id")]
    public int LoueurId { get; set; }

    [ForeignKey(nameof(LoueurId))]
    public Loueur? Loueur { get; set; }

    [Column("date_debut")]
    public DateOnly DateDebut { get; set; }

    [Column("date_fin")]
    public DateOnly DateFin { get; set; }

    [Column("annule")]
    public bool Annule { get; set; }

    [Column("paye")]
    public bool Paye { get; set; }
}
