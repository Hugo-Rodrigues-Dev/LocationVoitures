using System.ComponentModel;

namespace LocationVoitures.ApiService.Features.Loueurs.Requests;

public class UpdateBlacklistLoueurRequest
{
    [Description("Indique si le loueur doit etre blackliste ou rehabilite.")]
    public bool EstBlacklist { get; set; }
}
