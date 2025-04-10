namespace EmailClient.Controllers;

using EmailClient.Services.Contracts;
using EmailClient.ViewModels.Email;
using Microsoft.AspNetCore.Mvc;

[Route("/Email")]
public class EmailController(IInboxService inboxService) : Controller
{
    [HttpGet("Inbox")]
    public async Task<IActionResult> GetInbox(int page = 1)
    {
        var inboxResult = await inboxService.GetInboxAsync(HttpContext.Session.Id, page, 10); // this in constant TODO
        if (!inboxResult.IsSuccess)
        {
            return View("Error", inboxResult.Error!.Message);
        }

        var inboxVm = inboxResult.GetData<InboxViewModel>();
        return View("Inbox", inboxVm);
    }
}
