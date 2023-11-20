using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityServer.DBContext;
using IdentityServer.Services;

var builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;

string connectionString = "server=.;database=DbOpenid;Integrated Security=True;TrustServerCertificate=True";
services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connectionString));

services.AddIdentity<IdentityUser, IdentityRole>(o =>
{
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireDigit = false;
    o.Password.RequiredLength = 4;
    o.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

services.ConfigureApplicationCookie(o =>
{
    o.Cookie.Name = "IdentityServer.OrangeCookie";
    o.LoginPath = "/auth/Login";

});

var assembly = typeof(Program).Assembly.GetName().Name;
services.AddIdentityServer()
    .AddAspNetIdentity<IdentityUser>()
    .AddConfigurationStore(o =>
    {
        o.ConfigureDbContext = b =>
           b.UseSqlServer(connectionString,
               sql => sql.MigrationsAssembly(assembly));
    }).AddOperationalStore(o =>
    {
        o.ConfigureDbContext = b =>
           b.UseSqlServer(connectionString,
               sql => sql.MigrationsAssembly(assembly));
        //o.EnableTokenCleanup = true;
    }).AddDeveloperSigningCredential();


//services.AddIdentityServer()
//    .AddAspNetIdentity<IdentityUser>()
//    .AddInMemoryIdentityResources(Config.IdentityResources)
//    .AddInMemoryApiResources(Config.GetApisResource)
//    .AddInMemoryApiScopes(Config.GetApisScope)
//    .AddInMemoryClients(Config.GetClients)
//    .AddDeveloperSigningCredential();



services.AddControllersWithViews();

var app = builder.Build();
app.CreateUser();
app.UseRouting();
app.UseIdentityServer();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute(); 
});

app.Run();
