namespace EmailClient.Controllers;

using EmailClient.Domain.Results;
using EmailClient.Services.Contracts;
using EmailClient.ViewModels.Email;
using EmailClient.ViewModels.Email.Requests;
using Microsoft.AspNetCore.Mvc;

[Route("/Email")]
public class EmailController(IInboxService inboxService, IEmailSenderService emailSenderService) : Controller
{
    [HttpGet("Inbox")]
    public async Task<IActionResult> GetInbox(int page = 1, bool refresh = false)
    {
        var inboxResult = await inboxService.GetInboxAsync(HttpContext.Session.Id, page, 10, refresh); // this in constant TODO
        if (!inboxResult.IsSuccess)
        {
            return inboxResult.BuildResult();
        }

        var inboxVm = inboxResult.GetData<InboxViewModel>();
        return View("Inbox", inboxVm);
    }

    [HttpPost("SendEmail")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest sendEmailRequest)
    {
        var emailSendResult = await emailSenderService.SendEmailAsync(sendEmailRequest);
        return emailSendResult.BuildResult();
    }
}
