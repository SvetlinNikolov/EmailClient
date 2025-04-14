namespace EmailClient.ViewModels.ViewModels.Login;

public class ImapLoginViewModel
{
    public string ImapServer { get; set; } = string.Empty;
    public int ImapPort { get; set; }
    public string ImapUsername { get; set; } = string.Empty;
    public string ImapPassword { get; set; } = string.Empty;
}
