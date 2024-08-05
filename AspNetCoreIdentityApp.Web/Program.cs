using AspNetCoreIdentityApp.Web.ClaimProviders;
using AspNetCoreIdentityApp.Web.Context;
using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.OptionsModels;
using AspNetCoreIdentityApp.Web.PermissionsRoot;
using AspNetCoreIdentityApp.Web.Requirements;
using AspNetCoreIdentityApp.Web.Seeds;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using static AspNetCoreIdentityApp.Web.Requirements.ViolenceRequirement;

namespace AspNetCoreIdentityApp.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
            });

            builder.Services.Configure<SecurityStampValidatorOptions>(opt =>
            {
                opt.ValidationInterval = TimeSpan.FromMinutes(30);
            });

            //
            builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

            //Facebook ile Giriþ
            builder.Services.AddAuthentication()
            .AddFacebook(opt =>
            {
                opt.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
                opt.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;

            });

            builder.Services.AddIdentityWithExt();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, ViolenceRequirementHandler>();
            builder.Services.AddAuthorization(opt =>
            {
                opt.AddPolicy("AnkaraPolicy", policy =>
                {
                    policy.RequireClaim("City", "Ankara");
                    policy.RequireRole("admin");
                });

                opt.AddPolicy("ExchangePolicy", policy =>
                {
                    policy.AddRequirements(new ExchangeExpireRequirement());

                });

                opt.AddPolicy("ViolencePolicy", policy =>
                {
                    policy.AddRequirements(new ViolenceRequirement() { ThresholdAge = 18 });
                });

                opt.AddPolicy("OrderPermissionReadOrDeletePolicy", policy =>
                {
                    policy.RequireClaim("Permission", Permissions.Order.Read);
                    policy.RequireClaim("Permission", Permissions.Order.Delete);
                    policy.RequireClaim("Permission", Permissions.Stock.Delete);
                });

                opt.AddPolicy("Permissions.Order.Read", policy =>
                {
                    policy.RequireClaim("Permission", Permissions.Order.Read);
                });

                opt.AddPolicy("Permissions.Order.Delete", policy =>
                {
                    policy.RequireClaim("Permission", Permissions.Order.Delete);
                });

                opt.AddPolicy("Permissions.Stock.Delete", policy =>
                {
                    policy.RequireClaim("Permission", Permissions.Stock.Delete);
                });
            });


            builder.Services.ConfigureApplicationCookie(opt =>
            {
                var cookieBuilder = new CookieBuilder();
                cookieBuilder.Name = "UdemyAppCookie";
                opt.LoginPath = new PathString("/Home/SignIn");
                opt.LogoutPath = new PathString("/Member/LogOut");
                opt.AccessDeniedPath = new PathString("/Member/AccessDenied"); //eriþim reddedildiðinde yönlendirilecek sayfayý belirtir.
                opt.Cookie = cookieBuilder;
                opt.ExpireTimeSpan = TimeSpan.FromDays(60); // oturumun 60 gün oyunca geçerli olacaðýný belirtir.
                opt.SlidingExpiration = true; // oturum süresinin her istekte sýfýrlanacaðý ve pasif bir süre boyunca etkin olacaðý anlamýna gelir.
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
                await PermissionSeed.Seed(roleManager);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
               name: "areas",
               pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
