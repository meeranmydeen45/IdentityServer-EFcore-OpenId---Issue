using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
           new IdentityResources.OpenId(),
           new IdentityResources.Profile(),
           new IdentityResource()
           {
               Name = "apione.claims",
               UserClaims = { "user.role" }
           }
        };
        public static IEnumerable<ApiResource> GetApisResource =>
        new List<ApiResource>
        {
          new ApiResource("apione", new string[] { "user.role" })
          {
              Scopes = { "apione.read"},
          },
        };
        public static IEnumerable<ApiScope> GetApisScope =>
        new List<ApiScope>
        {
          new ApiScope("apione.read")
        };

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client{
                    ClientId = "client_service_1",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris ={ "https://localhost:5555/signin-oidc" },
                    //PostLogoutRedirectUris = { "https://localhost:5555/signout-callback-oidc" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "apione",
                        "apione.claims"
                    },
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    //AlwaysIncludeUserClaimsInIdToken = true,
                }
            };
        }
       
    }
}
