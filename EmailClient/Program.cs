using EmailClient.Factories.Contracts;
using EmailClient.Factories;
using EmailClient.Services.Contracts;
using EmailClient.Services;
using EmailClient.Auth;

namespace EmailClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddMemoryCache();
            builder.Services.AddDataProtection();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IImapClientFactory, ImapClientFactory>();
            builder.Services.AddSingleton<ISmtpClientFactory, SmtpClientFactory>();
            builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
            builder.Services.AddSingleton<ICookieAuthService, CookieAuthService>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IInboxService, InboxService>();
            builder.Services.AddSingleton<IPaginationService, PaginationService>();
            builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();
            builder.Services.AddSingleton<IIMapSessionService, IMapSessionService>();
            
            var app = builder.Build();
            app.UseHttpsRedirection();
            if (!app.Environment.IsDevelopment())
            {
                app.Urls.Add("http://+:8080");
            }

            app.UseStaticFiles();
            app.UseSession();
            app.Use((context, next) =>
            {
                context.Session.Set("Init", [1]); // triggers session creation
                return next();
            });
            app.UseRouting();
            app.UseAuthorization();

            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}
