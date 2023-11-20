using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using IdentityServer4.EntityFramework.Entities;
using System.Linq;


namespace IdentityServer.Services
{
    public static class UserService
    {
        public static void CreateUser(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var user = new IdentityUser("peter");
                userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
                userManager.AddClaimAsync(user, new Claim("user.role", "admin")).GetAwaiter().GetResult();

                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        Client c = new Client();
                        c.ClientId = client.ClientId;
                        c.ClientSecrets = client.ClientSecrets.Select(x => new ClientSecret { Value = x.Value }).ToList();
                        c.AllowedGrantTypes = client.AllowedGrantTypes.Select(x => new ClientGrantType { GrantType = x }).ToList();
                        c.RedirectUris = client.RedirectUris.Select(x => new ClientRedirectUri { RedirectUri = x }).ToList();
                        c.AllowedScopes = client.AllowedScopes.Select(x => new ClientScope { Scope = x }).ToList(); 
                        c.RequireConsent = client.RequireConsent;
                        c.AllowOfflineAccess =  client.AllowOfflineAccess;
                        context.Clients.Add(c);
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        IdentityResource identityResource = new IdentityResource();
                        identityResource.Name = resource.Name;
                        context.IdentityResources.Add(identityResource);
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApisResource)
                    {
                        ApiResource apiResource = new ApiResource();
                        apiResource.Name = resource.Name;
                        apiResource.UserClaims = resource.UserClaims.Select(x => new ApiResourceClaim {Type = x}).ToList();
                        apiResource.Scopes = resource.Scopes.Select(x => new ApiResourceScope { Scope = x }).ToList();
                        context.ApiResources.Add(apiResource);
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Config.GetApisScope)
                    {
                        ApiScope apiScope = new ApiScope();
                        apiScope.Name = resource.Name;
                        context.ApiScopes.Add(apiScope);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
