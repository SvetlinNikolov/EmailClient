namespace EmailClient.ViewModels.Login;

public class LoginViewModel
{
    public ImapLoginViewModel ImapLogin { get; set; } = new();

    public SmtpLoginViewModel SmtpLogin { get; set; } = new();

    public string? ErrorMessage { get; set; }
}
