using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.ClaimProviders;

public class UserClaimProvider : IClaimsTransformation
{

    private readonly UserManager<AppUser> _userManager;

    public UserClaimProvider(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    // City bilgisini dinamik olarak cookie içerisinde tutuyoruz.
    // DB'deki AspNetUserClaims tablosuna kayıt işlemi yapmıyoruz.
    // ClaimPrincial -> Kimlik doğrulama sonucunda elde edilen kimlik bilgilerini içeren bir nesnedir.
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identityUser = principal.Identity as ClaimsIdentity;
        var currentUser = await _userManager.FindByNameAsync(identityUser!.Name!);

        if (currentUser == null) return principal;
        if (string.IsNullOrEmpty(currentUser.City)) return principal;

        if (principal.HasClaim(x => x.Type != "City"))
        {
            Claim cityClaim = new Claim("City", currentUser.City);
            identityUser.AddClaim(cityClaim);
        }
        return principal;
    }
}
