namespace EmailClient.Services.Services.Contracts;

using EmailClient.ViewModels.ViewModels.Login;
using EmailService.Domain.Results;

public interface ILoginService
{
    Task<Result> LoginAsync(LoginViewModel loginViewModel);
    Result Logout();
}
