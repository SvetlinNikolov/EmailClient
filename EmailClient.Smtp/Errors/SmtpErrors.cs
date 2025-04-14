using EmailService.Domain.Errors;
using System.Net;

namespace EmailClient.Smtp.Errors;

public static class SmtpErrors
{
    public static Error ConnectionFailed(string? details = null) =>
         new("SMTP_CONNECTION_FAILED", "We couldn't connect to the email server. Please check the server address and port.", HttpStatusCode.BadGateway);

    public static Error AuthenticationFailed(string? details = null) =>
        new("SMTP_AUTH_FAILED", "Login to the email server failed. Please check your email and password.", HttpStatusCode.Unauthorized);

    public static Error InvalidMessage(string field) =>
        new("SMTP_INVALID_MESSAGE", $"The field '{field}' is required and cannot be empty.", HttpStatusCode.BadRequest);

    public static Error CommandFailed(string command, string? response = null) =>
        new("SMTP_COMMAND_FAILED", "There was an issue sending your email. Please try again.", HttpStatusCode.InternalServerError);

    public static Error UnexpectedResponse(string? response = null) =>
        new("SMTP_UNEXPECTED_RESPONSE", "Something went wrong while sending your message. Please try again later.", HttpStatusCode.InternalServerError);
}
