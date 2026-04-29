using LocationVoitures.Web.Features.Loueurs.Models;
using LocationVoitures.Web.Services;

namespace LocationVoitures.Web.Features.Loueurs.Services;

public class LoueursApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<LoueurDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("/loueurs", cancellationToken);
        await ApiResponseHelper.EnsureSuccessAsync(response, cancellationToken);
        return await ApiResponseHelper.ReadRequiredJsonAsync<List<LoueurDto>>(response, cancellationToken);
    }
}
