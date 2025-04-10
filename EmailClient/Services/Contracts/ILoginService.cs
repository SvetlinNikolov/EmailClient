namespace EmailClient.Services.Contracts;

using EmailClient.Domain.Results;
using EmailClient.ViewModels.Login;

public interface ILoginService
{
    Task<Result> LoginAsync(LoginViewModel loginViewModel);
}
