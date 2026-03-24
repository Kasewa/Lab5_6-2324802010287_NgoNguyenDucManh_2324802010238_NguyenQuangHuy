using ASC.Model.BaseTypes;
using ASC.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ASC.Web.Data
{
    public class IdentitySeed : IIdentitySeed
    {
        public async Task Seed(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<ApplicationSettings> options)
        {
            // Get all comma-separated roles
            var roles = options.Value.Roles.Split(new char[] { ',' });

            // Create roles if they don't exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Trim()))
                {
                    IdentityRole storageRole = new IdentityRole
                    {
                        Name = role.Trim()
                    };
                    IdentityResult roleResult = await roleManager.CreateAsync(storageRole);
                }
            }

            // Create admin if it doesn't exist
            var admin = await userManager.FindByEmailAsync(options.Value.AdminEmail);
            if (admin == null)
            {
                var user = new IdentityUser
                {
                    UserName = options.Value.AdminName,
                    Email = options.Value.AdminEmail,
                    EmailConfirmed = true
                };
                IdentityResult result = await userManager.CreateAsync(user, options.Value.AdminPassword);
                await userManager.AddClaimAsync(user, new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", options.Value.AdminEmail));
                await userManager.AddClaimAsync(user, new Claim("IsActive", "True"));

                // Assign Admin role
                await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }

            // Create a service engineer if it doesn't exist
            var engineer = await userManager.FindByEmailAsync(options.Value.EngineerEmail);
            if (engineer == null)
            {
                var user = new IdentityUser
                {
                    UserName = options.Value.EngineerName,
                    Email = options.Value.EngineerEmail,
                    EmailConfirmed = true
                };
                IdentityResult result = await userManager.CreateAsync(user, options.Value.EngineerPassword);
                await userManager.AddClaimAsync(user, new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", options.Value.EngineerEmail));
                await userManager.AddClaimAsync(user, new Claim("IsActive", "True"));

                // Assign Engineer role
                await userManager.AddToRoleAsync(user, Roles.Engineer.ToString());
            }
        }
    }
}
