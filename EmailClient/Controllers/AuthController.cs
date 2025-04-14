namespace EmailClient.Controllers;

using EmailClient.Services.Services.Contracts;
using EmailClient.ViewModels.ViewModels.Login;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
public class AuthController(ILoginService loginService) : Controller
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        var loginResult = await loginService.LoginAsync(loginViewModel);

        if (!loginResult.IsSuccess)
        {
            loginViewModel.ErrorMessage = loginResult.Error?.Message;
            loginViewModel.ErrorCode = loginResult.Error?.Code;
            return View(loginViewModel);
        }

        return RedirectToAction("Inbox", "Email");
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        var logout = loginService.Logout();
        return RedirectToAction("Login");
    }
}
