using LocationVoitures.Web.Features.Voitures.Models;
using LocationVoitures.Web.Services;

namespace LocationVoitures.Web.Features.Voitures.Services;

public class VoituresApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<VoitureDto>> GetCatalogueAsync(string? categorie = null, CancellationToken cancellationToken = default)
    {
        var uri = string.IsNullOrWhiteSpace(categorie)
            ? "/voitures"
            : $"/voitures?categorie={Uri.EscapeDataString(categorie)}";

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        return await ApiResponseHelper.ReadRequiredJsonAsync<List<VoitureDto>>(response, cancellationToken);
    }

    public async Task<VoitureDto> GetByImmatriculationAsync(string immatriculation, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"/voitures/immatriculation/{Uri.EscapeDataString(immatriculation)}", cancellationToken);
        await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        return await ApiResponseHelper.ReadRequiredJsonAsync<VoitureDto>(response, cancellationToken);
    }
}
