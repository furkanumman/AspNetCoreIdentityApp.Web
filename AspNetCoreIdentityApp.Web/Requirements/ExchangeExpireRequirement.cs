using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentityApp.Web.Requirements;

/// <summary>
/// Business işlemleri yaptığımız parametreleri geçebilmek için kullanıyoruz.
/// </summary>
public class ExchangeExpireRequirement : IAuthorizationRequirement
{

}

/// <summary>
/// Business işlemlerini yaptığımız handler sınıfımız.
/// </summary>
public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
    {
        var hasExchangeExpireClaim = context.User.HasClaim(x => x.Type == "ExchangeExpireDate");

        if (!hasExchangeExpireClaim)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate");

        if (DateTime.Now > Convert.ToDateTime(exchangeExpireDate!.Value))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}