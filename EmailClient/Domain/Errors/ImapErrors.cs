namespace EmailClient.Domain.Errors;

public static class ImapErrors
{
    public static Error ImapConnectionFailed(string? details = null) =>
        new("IMAP_CONNECTION_FAILED", "Unable to connect to the email server. Please check the server address and port.");

    public static Error ImapLoginFailed(string? serverResponse = null) =>
        new("IMAP_LOGIN_FAILED", "Login failed. Please check your email and password.");

    public static Error ImapLogoutFailed(string? serverResponse = null) =>
        new("IMAP_LOGOUT_FAILED", "Logout from the email server failed.");

    public static Error ImapListMailboxesFailed(string? details = null) =>
        new("IMAP_LIST_FAILED", "Unable to retrieve mailbox folders.");

    public static Error ImapNoTaggedResponse() =>
        new("IMAP_NO_TAGGED_RESPONSE", "The email server did not respond as expected.");

    public static Error ImapWriterNotInitialized() =>
        new("IMAP_WRITER_NOT_READY", "The connection is not fully initialized. Please try reconnecting.");

    public static Error ImapReaderNotInitialized() =>
        new("IMAP_READER_NOT_READY", "The connection is not fully initialized. Please try reconnecting.");
}
