namespace EmailClient.ViewModels.Email;

public class EmailHeader(string subject, string from, string date)
{
    public string Subject { get; set; } = subject;
    public string From { get; set; } = from;
    public string Date { get; set; } = date;
}
