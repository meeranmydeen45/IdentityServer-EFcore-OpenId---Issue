using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

services.AddAuthentication(o =>
{
    o.DefaultScheme = "cookie";
    o.DefaultChallengeScheme = "oidc";
})
    .AddCookie("cookie")
    .AddOpenIdConnect("oidc", o =>
    {
        o.Authority = "https://localhost:4000/";
        o.ClientId = "client";
        o.ClientSecret = "secret";
        o.SaveTokens = true;
        o.ResponseType = "code";

        o.ClaimActions.DeleteClaim("amr");
        o.ClaimActions.MapUniqueJsonKey("orange.cookie", "user.role");

        o.GetClaimsFromUserInfoEndpoint = true;
        o.Scope.Add("apione.claims");
        o.Scope.Add("apione.read");

    });

services.AddHttpContextAccessor();
services.AddControllersWithViews();

var app = builder.Build();


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); 
});
app.Run();