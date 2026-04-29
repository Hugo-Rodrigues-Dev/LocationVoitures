namespace LocationVoitures.Web.Features.Locations.Models;

public class ReserverResponse
{
    public int IdLocation { get; set; }

    public decimal PrixTotal { get; set; }

    public string Message { get; set; } = string.Empty;
}
