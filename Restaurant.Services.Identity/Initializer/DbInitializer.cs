using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Restaurant.Services.Identity.DbContexts;
using Restaurant.Services.Identity.Models;
using System.Security.Claims;

namespace Restaurant.Services.Identity.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = applicationDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(StaticDetails.Admin).Result == null)
            {
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(StaticDetails.Customer)).GetAwaiter().GetResult();

                ApplicationUser adminUser = new()
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    PhoneNumber = "11111",
                    FirstName = "The",
                    LastName = "Admin"
                };

                _userManager.CreateAsync(adminUser, "Admin456*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(adminUser, StaticDetails.Admin).GetAwaiter().GetResult();
                _ = _userManager.AddClaimsAsync(adminUser, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, adminUser.FirstName + " " + adminUser.LastName),
                    new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
                    new Claim(JwtClaimTypes.Role, StaticDetails.Admin),
                }).Result;

                ApplicationUser customerUser = new()
                {
                    UserName = "customer@customer.com",
                    Email = "customer@customer.com",
                    EmailConfirmed = true,
                    PhoneNumber = "11111",
                    FirstName = "The",
                    LastName = "Customer"
                };

                _userManager.CreateAsync(customerUser, "Customer456*").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(customerUser, StaticDetails.Customer).GetAwaiter().GetResult();
                _ = _userManager.AddClaimsAsync(customerUser, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, customerUser.FirstName + " " + customerUser.LastName),
                    new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
                    new Claim(JwtClaimTypes.Role, StaticDetails.Customer),
                }).Result;
            }
        }

    }
}
