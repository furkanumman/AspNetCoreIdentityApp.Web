using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.Services;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInVM model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(model.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya þifre yanlýþ!");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giriþ yapamazsýnýz" });
                return View();
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelErrorList(new List<string>() { "Email veya þifre yanlýþ!", $"Baþarýsýz giriþ sayýsý: {await _userManager.GetAccessFailedCountAsync(hasUser)}" });
                return View();
            }

            if (hasUser.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(hasUser, model.RememberMe, new[] { new Claim("BirthDate", hasUser.BirthDate.Value.ToString()) });
            }
            return Redirect(returnUrl!);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpVM request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var identityResult = await _userManager.CreateAsync(new()
            {
                UserName = request.UserName,
                PhoneNumber = request.Phone,
                Email = request.Email
            }, request.PasswordConfirm);

            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                return View();
            }

            // Claim nesnesi oluþturuluyor.
            var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());

            // Kayýt olan kullanýcý bulunuyor.
            var user = await _userManager.FindByNameAsync(request.UserName);

            //Claim  ekleniyor.
            var claimResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaim);

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());
                return View();
            }

            TempData["SuccessMessage"] = "Üyelik Kayýt iþlemi baþarýyla gerçekleþmiþtir.";
            return RedirectToAction(nameof(HomeController.SignUp));
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM request)
        {
            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Bu e-mail adresine sahip kullanýcý bulunamamýþtýr!");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);
            string passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            // örnek link https://localhost

            // EmailService  // giqi auwg buqt mupf

            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);

            TempData["SuccessMessage"] = "Þifre yenileme linki e-mail adresinize gönderilmiþtir!";

            return RedirectToAction(nameof(ForgetPassword));
        }


        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                throw new Exception("Bir hata meydana geldi!");
            }

            var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);
            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanýcý bulunamamýþtýr!");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Þifreniz baþarýyla yenilenmiþtir!";
            }
            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
            }
            return View();
        }



        public IActionResult FacebookSignIn(string returnUrl)
        {
            // Facebook'ta giriþ yaptýktan sonra döneceði URL'i tanýmlýyoruz. (Son parametre query string parametresidir.)
            var redirectUrl = Url.Action("ExternalResponse", "Home", new { returnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", properties);
        }


        public async Task<IActionResult> ExternalResponse(string returnUrl = "/")
        {
            // Login olan kullanýcaya ait bilgileri alýyoruz.
            ExternalLoginInfo info = (await _signInManager.GetExternalLoginInfoAsync())!;

            if (info == null)
            {
                return RedirectToAction("SignIn");
            }
            else
            {
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

                if (result.Succeeded)
                {
                    return RedirectToAction(returnUrl);
                }
                else
                {
                    AppUser user = new AppUser();

                    user.Email = info.Principal.FindFirst(ClaimTypes.Email)!.Value;
                    string externalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;

                    if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
                    {
                        string userName = info.Principal.FindFirst(ClaimTypes.Name)!.Value;
                        userName = userName.Replace(' ', '_').ToLower() + externalUserId.Substring(0, 5).ToString();
                        user.UserName = userName;
                    }
                    else
                        user.UserName = info.Principal.FindFirst(ClaimTypes.Email)!.Value;

                    var existUser = await _userManager.FindByEmailAsync(user.Email);

                    if (existUser == null)
                    {
                        var createResult = await _userManager.CreateAsync(user);

                        if (createResult.Succeeded)
                        {
                            // AspNetUserLogins tablosu dolduruluyor!
                            var loginResult = await _userManager.AddLoginAsync(user, info);

                            if (loginResult.Succeeded)
                            {
                                //await _signInManager.SignInAsync(user, true);
                                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                                return Redirect(returnUrl);
                            }

                            ModelState.AddModelErrorList(loginResult.Errors);
                        }
                        ModelState.AddModelErrorList(createResult.Errors);
                    }
                    else
                    {
                        var loginResult = await _userManager.AddLoginAsync(existUser, info);
                        await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                        return Redirect(returnUrl);
                    }
                }
            }

            return RedirectToAction("Error");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
