using AspNetCoreIdentityApp.Web.Context;
using AspNetCoreIdentityApp.Web.CustomValidations;
using AspNetCoreIdentityApp.Web.Localizations;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Extensions;

public static class StartupExtensions
{
    public static void AddIdentityWithExt(this IServiceCollection services)
    {
        services.Configure<DataProtectionTokenProviderOptions>(opt =>
        {
            opt.TokenLifespan = TimeSpan.FromHours(2);
        });

        services.AddIdentity<AppUser, AppRole>(opt =>
        {
            opt.User.RequireUniqueEmail = true;

            //opt.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwyz123456789_";

            opt.Password.RequiredLength = 6;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireLowercase = true;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireDigit = false;

            opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
            opt.Lockout.MaxFailedAccessAttempts = 3;

        }).AddPasswordValidator<PasswordValidator>()
          .AddUserValidator<UserValidator>()
          .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
          .AddDefaultTokenProviders()
          .AddEntityFrameworkStores<AppDbContext>();
    }
}
