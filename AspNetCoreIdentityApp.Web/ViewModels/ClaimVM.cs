namespace AspNetCoreIdentityApp.Web.ViewModels;

public class ClaimVM
{
    public string Issuer { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Value { get; set; } = null!;
}
