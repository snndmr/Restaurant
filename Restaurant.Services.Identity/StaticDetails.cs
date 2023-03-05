using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Restaurant.Services.Identity
{
    public static class StaticDetails
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("restaurant", "Restaurant Server"),
                new ApiScope("read", "Read your data."),
                new ApiScope("write", "Write your data."),
                new ApiScope("delete", "Delete your data."),
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "read", "write", "profile" }
                },
                new Client
                {
                    ClientId = "restaurant",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:44345/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44345/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "restaurant"
                    }
                }
            };
    }
}
