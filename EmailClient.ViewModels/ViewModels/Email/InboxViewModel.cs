namespace EmailClient.ViewModels.ViewModels.Email;

public class InboxViewModel
{
    public IEnumerable<EmailHeader>? Emails { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }
}
