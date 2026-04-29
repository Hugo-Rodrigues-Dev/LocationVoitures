using LocationVoitures.Web.Features.Locations.Models;
using LocationVoitures.Web.Services;
using System.Net.Http.Json;

namespace LocationVoitures.Web.Features.Locations.Services;

public class LocationsApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<LocationDto>> GetByVoitureAsync(string immatriculation, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"/locations/voiture/{Uri.EscapeDataString(immatriculation)}", cancellationToken);
        await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        return await ApiResponseHelper.ReadRequiredJsonAsync<List<LocationDto>>(response, cancellationToken);
    }

    public async Task<ReserverResponse> CreateAsync(ReserverRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("/locations", request, cancellationToken);
        await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        return await ApiResponseHelper.ReadRequiredJsonAsync<ReserverResponse>(response, cancellationToken);
    }
}
