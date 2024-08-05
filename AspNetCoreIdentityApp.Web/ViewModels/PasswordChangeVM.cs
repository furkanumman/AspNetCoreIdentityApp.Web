using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels;

public class PasswordChangeVM
{
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Eski Parola alanı boş bırakılamaz.")]
    [Display(Name = "Eski Parola:")]
    [MinLength(6, ErrorMessage = "Parola en az 6 karakter olabilir.")]
    public string PasswordOld { get; set; } = null!;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Yeni Parola alanı boş bırakılamaz.")]
    [Display(Name = "Yeni Parola:")]
    [MinLength(6, ErrorMessage = "Parola en az 6 karakter olabilir.")]
    public string PasswordNew { get; set; } = null!;


    [DataType(DataType.Password)]
    [Compare(nameof(PasswordNew), ErrorMessage = "Parola aynı değildir.")]
    [Required(ErrorMessage = "Yeni Parola tekrar alanı boş bırakılamaz.")]
    [Display(Name = "Yeni Parola Tekrar:")]
    [MinLength(6, ErrorMessage = "Parola en az 6 karakter olabilir.")]
    public string PasswordNewConfirm { get; set; } = null!;

}
