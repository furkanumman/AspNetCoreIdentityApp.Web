using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentityApp.Web.Requirements;

public class ViolenceRequirement : IAuthorizationRequirement
{
    public int ThresholdAge { get; set; }

    public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
        {
            var hasExchangeExpireClaim = context.User.HasClaim(x => x.Type == "BirthDate");

            if (!hasExchangeExpireClaim)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var birthDateClaim = context.User.FindFirst("BirthDate");

            var today = DateTime.Now;
            var birthDate = Convert.ToDateTime(birthDateClaim!.Value);

            var age = today.Year - Convert.ToDateTime(birthDateClaim!.Value).Year;
            
            // Artık yıl hesaplaması
            if (birthDate > today.AddYears(-age)) age--;

            if (requirement.ThresholdAge > age)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
