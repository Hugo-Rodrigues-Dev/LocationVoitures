using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LocationVoitures.ApiService.Features.Voitures.Requests;

public class UpdatePrixVoitureRequest
{
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    [Description("Nouveau prix de location journalier. Doit etre strictement positif.")]
    public decimal PrixLocationParJour { get; set; }
}
