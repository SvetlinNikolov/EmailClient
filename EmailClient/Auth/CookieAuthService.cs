namespace EmailClient.Auth;

using EmailClient.Domain.Constants;
using EmailClient.Domain.Errors;
using EmailClient.Domain.Results;
using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;

public class CookieAuthService : ICookieAuthService
{
    private readonly IDataProtector _protector;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieAuthService(IDataProtectionProvider provider, IHttpContextAccessor httpContextAccessor)
    {
        _protector = provider.CreateProtector(nameof(LoginCookie));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(IHttpContextAccessor));
    }

    public Result SaveLoginCookie(LoginCookie payload)
    {
        try
        {
            var json = JsonSerializer.Serialize(payload);
            var protectedData = _protector.Protect(json);

            _httpContextAccessor.HttpContext!.Response.Cookies.Append(GetCookieKey(), protectedData, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.Add(CookieConstants.AuthCookieTtl),
                SameSite = SameSiteMode.Strict
            });

            return Result.Success(payload);
        }
        catch
        {
            return Result.Failure(ImapErrors.ImapLogoutFailed());
        }
    }

    public Result GetLoginCookie()
    {
        try
        {
            if (!_httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue(GetCookieKey(), out var raw))
                return Result.Failure(CookieErrors.CookieNotFound());

            var json = _protector.Unprotect(raw);
            var payload = JsonSerializer.Deserialize<LoginCookie>(json);

            return payload != null
                ? Result.Success(payload)
                : Result.Failure(CookieErrors.CookieCorrupted());
        }
        catch
        {
            return Result.Failure(CookieErrors.CookieDeserializationFailed());
        }
    }

    public void ClearLoginCookie()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(GetCookieKey());
    }

    public Result IsLoggedIn()
    {
        var result = GetLoginCookie();
        return result.IsSuccess
            ? Result.Success()
            : Result.Failure(result.Error!);
    }

    private string GetCookieKey() =>
    $"{CookieConstants.AuthCookieKey}_{_httpContextAccessor.HttpContext?.Session.Id}";
}
