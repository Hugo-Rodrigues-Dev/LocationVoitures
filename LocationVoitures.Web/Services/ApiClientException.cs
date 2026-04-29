using System.Net;

namespace LocationVoitures.Web.Services;

public sealed class ApiClientException : Exception
{
    public ApiClientException(HttpStatusCode statusCode, string message, string? problemTitle = null)
        : base(message)
    {
        StatusCode = statusCode;
        ProblemTitle = problemTitle;
    }

    public HttpStatusCode StatusCode { get; }

    public string? ProblemTitle { get; }
}
