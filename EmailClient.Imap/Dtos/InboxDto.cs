using EmailClient.Imap.Dtos;

namespace EmailClient.Imap.Models;

public class InboxDto
{
    public IEnumerable<EmailHeaderDto>? Emails { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }
}
