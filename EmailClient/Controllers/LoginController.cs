namespace EmailClient.Controllers;
using EmailClient.Services.Contracts;
using EmailClient.ViewModels.Login;
using Microsoft.AspNetCore.Mvc;

[Route("/Login")]
public class LoginController(ILoginService loginService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        var loginResult = await loginService.LoginAsync(loginViewModel);

        if (!loginResult.IsSuccess)
        {
            loginViewModel.ErrorMessage = loginResult.Error?.Message;
            return View(loginViewModel);
        }

        return RedirectToAction("Inbox", "Email");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }
}
