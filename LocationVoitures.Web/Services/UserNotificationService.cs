using System.Net;

namespace LocationVoitures.Web.Services;

public sealed class UserNotificationService
{
    public event Action? OnChange;

    public NotificationMessage? Current { get; private set; }

    public void ShowSuccess(string title, string message)
        => Show(NotificationSeverity.Success, title, message);

    public void ShowWarning(string title, string message)
        => Show(NotificationSeverity.Warning, title, message);

    public void ShowError(string title, string message)
        => Show(NotificationSeverity.Error, title, message);

    public void ShowException(Exception exception, string fallbackTitle)
    {
        switch (exception)
        {
            case ApiClientException apiException:
                Show(MapSeverity(apiException.StatusCode),
                    MapTitle(apiException, fallbackTitle),
                    apiException.Message);
                break;

            case HttpRequestException:
                Show(NotificationSeverity.Error,
                    "Service indisponible",
                    "La gateway ou l'API ne repond pas pour le moment. Verifiez qu'Aspire et les services sont bien demarres.");
                break;

            default:
                Show(NotificationSeverity.Error,
                    fallbackTitle,
                    "Une erreur inattendue est survenue. Rechargez la page ou reessayez dans quelques instants.");
                break;
        }
    }

    public void Dismiss()
    {
        Current = null;
        OnChange?.Invoke();
    }

    private void Show(NotificationSeverity severity, string title, string message)
    {
        Current = new NotificationMessage(severity, title, message);
        OnChange?.Invoke();
    }

    private static NotificationSeverity MapSeverity(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => NotificationSeverity.Warning,
            HttpStatusCode.NotFound => NotificationSeverity.Warning,
            HttpStatusCode.Conflict => NotificationSeverity.Warning,
            _ when (int)statusCode >= 500 => NotificationSeverity.Error,
            _ => NotificationSeverity.Error
        };
    }

    private static string MapTitle(ApiClientException exception, string fallbackTitle)
    {
        if (!string.IsNullOrWhiteSpace(exception.ProblemTitle))
        {
            return exception.ProblemTitle!;
        }

        return exception.StatusCode switch
        {
            HttpStatusCode.BadRequest => "Saisie invalide",
            HttpStatusCode.NotFound => "Element introuvable",
            HttpStatusCode.Conflict => "Conflit de reservation",
            HttpStatusCode.Forbidden => "Action interdite",
            _ when (int)exception.StatusCode >= 500 => "Erreur serveur",
            _ => fallbackTitle
        };
    }
}

public sealed record NotificationMessage(NotificationSeverity Severity, string Title, string Message);

public enum NotificationSeverity
{
    Success,
    Warning,
    Error
}
