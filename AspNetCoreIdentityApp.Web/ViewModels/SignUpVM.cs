using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class SignUpVM
{
    public SignUpVM() { }
    public SignUpVM(string userName, string password, string email, string phone)
    {
        UserName = userName;
        Password = password;
        Email = email;
        Phone = phone;
    }

    [Required(ErrorMessage = "Kullanıcı ad alanı boş bırakılamaz.")]
    [Display(Name = "Kullanıcı Adı:")]
    public string UserName { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
    [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
    [Display(Name = "Email:")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Telefon alanı boş bırakılamaz.")]
    [Display(Name = "Telefon:")]
    public string Phone { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Parola alanı boş bırakılamaz.")]
    [Display(Name = "Parola:")]
    [MinLength(6, ErrorMessage = "Parola en az 6 karakter olabilir.")]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Parola aynı değildir.")]
    [Required(ErrorMessage = "Parola tekrar alanı boş bırakılamaz.")]
    [Display(Name = "Parola Tekrar:")]
    [MinLength(6, ErrorMessage = "Parola en az 6 karakter olabilir.")]
    public string PasswordConfirm { get; set; } = null!;

}

