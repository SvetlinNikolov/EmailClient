namespace EmailClient.ViewModels.Email;

public class InboxViewModel
{
    public List<EmailHeader> Emails { get; set; } = new();

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }
}
