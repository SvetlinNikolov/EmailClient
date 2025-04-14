namespace EmailClient.ViewModels.ViewModels.Email.Requests;

using System.ComponentModel.DataAnnotations;

public class SendEmailRequest
{
    [Required]
    public string To { get; set; }

    [Required]
    public string Subject { get; set; }

    [Required]
    public string Body { get; set; }
}
