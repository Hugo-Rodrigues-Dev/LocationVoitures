using LocationVoitures.Web.Features.Loueurs.Models;
using LocationVoitures.Web.Services;
using System.Net.Http.Json;

namespace LocationVoitures.Web.Features.Loueurs.Services;

public class LoueursApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<LoueurDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("/loueurs", cancellationToken);
        await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        return await ApiResponseHelper.ReadRequiredJsonAsync<List<LoueurDto>>(response, cancellationToken);
    }

    public async Task<LoueurDto> CreateAsync(CreateLoueurRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("/loueurs", request, cancellationToken);
        await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        return await ApiResponseHelper.ReadRequiredJsonAsync<LoueurDto>(response, cancellationToken);
    }

    public async Task<LoueurDto> UpdateBlacklistAsync(int loueurId, bool estBlacklist, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PatchAsJsonAsync(
            $"/loueurs/{loueurId}/blacklist",
            new UpdateBlacklistLoueurRequest { EstBlacklist = estBlacklist },
            cancellationToken);
        await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        return await ApiResponseHelper.ReadRequiredJsonAsync<LoueurDto>(response, cancellationToken);
    }
}
