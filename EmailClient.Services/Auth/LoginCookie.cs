namespace EmailClient.Services.Auth;

public class LoginCookie
{
    public string ImapServer { get; set; } = string.Empty;
    public int ImapPort { get; set; }
    public string ImapUsername { get; set; } = string.Empty;
    public string ImapPassword { get; set; } = string.Empty;

    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
}
