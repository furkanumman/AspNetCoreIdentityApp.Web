using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class ResetPasswordVM
{
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Parola alanı boş bırakılamaz.")]
    [Display(Name = "Yeni Parola:")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Parola aynı değildir.")]
    [Required(ErrorMessage = "Parola tekrar alanı boş bırakılamaz.")]
    [Display(Name = "Yeni Parola Tekrar:")]
    public string PasswordConfirm { get; set; } = null!;

}
