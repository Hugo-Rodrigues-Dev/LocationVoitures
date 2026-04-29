using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace LocationVoitures.Web.Services;

public static class ApiResponseHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task<T> ReadRequiredJsonAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return result ?? throw new InvalidOperationException("La reponse de l'API est vide.");
    }

    public static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var contentType = response.Content.Headers.ContentType?.MediaType;
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (contentType?.Contains("application/problem+json", StringComparison.OrdinalIgnoreCase) == true ||
            contentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true)
        {
            var validationProblem = JsonSerializer.Deserialize<ValidationProblemDetails>(body, JsonOptions);
            if (validationProblem?.Errors?.Count > 0)
            {
                throw new ApiClientException(response.StatusCode,
                    string.Join(" ", validationProblem.Errors.SelectMany(item => item.Value)),
                    validationProblem.Title);
            }

            var problem = JsonSerializer.Deserialize<ProblemDetails>(body, JsonOptions);
            if (!string.IsNullOrWhiteSpace(problem?.Detail))
            {
                throw new ApiClientException(response.StatusCode, problem.Detail, problem.Title);
            }

            if (!string.IsNullOrWhiteSpace(problem?.Title))
            {
                throw new ApiClientException(response.StatusCode, problem.Title, problem.Title);
            }
        }

        if (!string.IsNullOrWhiteSpace(body))
        {
            throw new ApiClientException(response.StatusCode, body);
        }

        throw new ApiClientException(response.StatusCode, $"L'API a renvoye le statut {(int)response.StatusCode}.");
    }
}
