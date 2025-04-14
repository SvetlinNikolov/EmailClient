namespace EmailClient.Imap.Dtos;

public class EmailHeaderDto(string subject, string from, string date)
{
    public string Subject { get; set; } = subject;
    public string From { get; set; } = from;
    public string Date { get; set; } = date;
}
