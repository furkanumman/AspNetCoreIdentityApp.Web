using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Models;

public class RoleCreateVM
{
    [Required(ErrorMessage = "Rol isim alanı boş bırakılamaz!")]
    [Display(Name = "Rol İsim:")]
    public string Name { get; set; } = null!;
}
