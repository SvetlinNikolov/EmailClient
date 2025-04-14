namespace EmailService.Domain.Errors;

using System.Net;

public static class CookieErrors
{
    public static Error CookieNotFound() =>
         new("COOKIE_NOT_FOUND", "Authentication cookie is missing. Please try logging in again.", HttpStatusCode.Unauthorized);

    public static Error CookieCorrupted() =>
        new("COOKIE_CORRUPTED", "The authentication cookie is corrupted or invalid. Please try logging in again.", HttpStatusCode.BadRequest);

    public static Error CookieDeserializationFailed() =>
        new("COOKIE_DESERIALIZATION_FAILED", "Failed to read login data from cookie. Please try logging in again.", HttpStatusCode.BadRequest);

    public static Error AlreadyLoggedIn() =>
         new("ALREADY_LOGGED_IN", "User already logged in.", HttpStatusCode.Conflict);
}
