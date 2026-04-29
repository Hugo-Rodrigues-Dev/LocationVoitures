using System.Net.Http.Json;
using LocationVoitures.ApiService.Features.Paiements.Requests;
using LocationVoitures.ApiService.Features.Paiements.Responses;

namespace LocationVoitures.ApiService.Features.Paiements.Services;

public class PaiementApiClient(HttpClient httpClient)
{
    public async Task<AutoriserPaiementResponse> AutoriserAsync(AutoriserPaiementRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("/payments/authorize", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AutoriserPaiementResponse>(cancellationToken);
        return payload ?? throw new InvalidOperationException("La reponse du service de paiement est vide.");
    }
}
