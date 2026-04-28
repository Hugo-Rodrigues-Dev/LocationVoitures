using System.ComponentModel;

namespace LocationVoitures.ApiService.Features.Loueurs;

public class LoueurDto
{
    [Description("Identifiant unique du loueur.")]
    public int Id { get; init; }

    [Description("Nom de famille du loueur.")]
    public string Nom { get; init; } = string.Empty;

    [Description("Prenom du loueur.")]
    public string Prenom { get; init; } = string.Empty;

    [Description("Numero de mobile unique du loueur.")]
    public string Mobile { get; init; } = string.Empty;

    [Description("Adresse email du loueur.")]
    public string? Email { get; init; }

    [Description("Pays de rattachement du loueur.")]
    public string Pays { get; init; } = string.Empty;

    [Description("Indique si le loueur est blackliste et ne peut plus reserver.")]
    public bool EstBlacklist { get; init; }
}
