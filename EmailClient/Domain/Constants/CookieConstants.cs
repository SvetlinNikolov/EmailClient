namespace EmailClient.Domain.Constants;

public static class CookieConstants
{
    public const string AuthCookieKey = "auth";
    public static readonly TimeSpan AuthCookieTtl = TimeSpan.FromMinutes(30);
}
