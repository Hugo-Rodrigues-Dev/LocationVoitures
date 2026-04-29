namespace LocationVoitures.Web.Features.Loueurs.Models;

public class LoueurDto
{
    public int Id { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Prenom { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string Pays { get; set; } = string.Empty;

    public bool EstBlacklist { get; set; }
}
