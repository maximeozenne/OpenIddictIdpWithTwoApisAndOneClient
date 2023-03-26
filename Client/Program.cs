using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using OpenIddict.Client;
using OpenIddict.Validation.AspNetCore;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.SignInScheme = "Cookies";
    options.Authority = "https://localhost:5443";
    options.ClientId = "client";
    options.ClientSecret = "client-secret";
    options.CallbackPath = "/authentication/signin-oidc";
    options.ResponseType = "code"; // Utilisation de l'Authorization Code Flow
    options.Scope.Add("profile");
    options.Scope.Add("standaloneapi");
    options.Scope.Add("dependentapi");

    options.SaveTokens = true;

    options.UsePkce = true;
    options.GetClaimsFromUserInfoEndpoint = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();