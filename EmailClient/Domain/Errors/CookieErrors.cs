namespace EmailClient.Domain.Errors;

public static class CookieErrors
{
    public static Error CookieNotFound() =>
        new("COOKIE_NOT_FOUND", "Authentication cookie is missing.");

    public static Error CookieCorrupted() =>
        new("COOKIE_CORRUPTED", "The authentication cookie is corrupted or invalid.");

    public static Error CookieDeserializationFailed() =>
        new("COOKIE_DESERIALIZATION_FAILED", $"Failed to read login data from cookie.");
}
