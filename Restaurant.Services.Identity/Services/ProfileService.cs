using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Restaurant.Services.Identity.Models;
using System.Data;
using System.Security.Claims;

namespace Restaurant.Services.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfileService(IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string subjectId = context.Subject.GetSubjectId();
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(subjectId);
            ClaimsPrincipal claimsPrincipal = await _userClaimsPrincipalFactory.CreateAsync(applicationUser);
            List<Claim> claims = claimsPrincipal.Claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

            claims.Add(new Claim(JwtClaimTypes.FamilyName, applicationUser.LastName ?? string.Empty));
            claims.Add(new Claim(JwtClaimTypes.GivenName, applicationUser.FirstName ?? string.Empty));

            if (_userManager.SupportsUserRole)
            {
                IList<string> roles = await _userManager.GetRolesAsync(applicationUser);
                foreach (string role in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, role));

                    if (_roleManager.SupportsRoleClaims)
                    {
                        var identityRole = await _roleManager.FindByNameAsync(role);

                        if (identityRole != null)
                        {
                            claims.AddRange(await _roleManager.GetClaimsAsync(identityRole));
                        }
                    }
                }
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string subjectId = context.Subject.GetSubjectId();
            ApplicationUser applicationUser = await _userManager.FindByIdAsync(subjectId);
            context.IsActive = applicationUser != null;
        }
    }
}
