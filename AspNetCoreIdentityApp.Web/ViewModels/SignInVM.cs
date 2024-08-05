using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class SignInVM
{
    public SignInVM() { }
    public SignInVM(string email, string password)
    {
        Email = email;
        Password = password;
    }

    [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
    [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
    [Display(Name = "Email:")]
    public string Email { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Parola alanı boş bırakılamaz.")]
    [Display(Name = "Parola:")]
    [MinLength(6, ErrorMessage = "Parola en az 6 karakter olabilir.")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Display(Name = "Beni Hatırla")]
    public bool RememberMe { get; set; }

}
