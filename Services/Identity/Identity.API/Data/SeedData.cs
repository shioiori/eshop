using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using AppRoles = Identity.API.Models.Roles;
using AppPermissions = Identity.API.Models.Permissions;
using Oidc = OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.API.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await SeedRolesAndPermissionsAsync(services);
        await SeedDefaultUsersAsync(services);
        await SeedOpenIddictClientsAsync(services);
    }

    private static async Task SeedRolesAndPermissionsAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        var roles = new[]
        {
            new ApplicationRole { Name = AppRoles.Admin, Description = "Full system access" },
            new ApplicationRole { Name = AppRoles.Customer, Description = "Standard customer access" },
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
                await roleManager.CreateAsync(role);
        }

        // Assign permissions to roles
        var dbContext = services.GetRequiredService<ApplicationDbContext>();

        await SeedRolePermissionsAsync(dbContext, roleManager, AppRoles.Admin, AppRoles.AdminPermissions);
        await SeedRolePermissionsAsync(dbContext, roleManager, AppRoles.Customer, AppRoles.CustomerPermissions);

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedRolePermissionsAsync(
        ApplicationDbContext dbContext,
        RoleManager<ApplicationRole> roleManager,
        string roleName,
        IReadOnlyList<string> permissions)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null) return;

        if (dbContext.RolePermissions.Any(p => p.RoleId == role.Id)) return;

        dbContext.RolePermissions.AddRange(
            permissions.Select(p => new ApplicationRolePermission { RoleId = role.Id, Permission = p })
        );
    }

    private static async Task SeedDefaultUsersAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var adminUser = new ApplicationUser
        {
            UserName = "admin@eshop.com",
            Email = "admin@eshop.com",
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true
        };

        if (await userManager.FindByEmailAsync(adminUser.Email) == null)
        {
            await userManager.CreateAsync(adminUser, "Admin@123456");
            await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
        }
    }

    private static async Task SeedOpenIddictClientsAsync(IServiceProvider services)
    {
        var manager = services.GetRequiredService<IOpenIddictApplicationManager>();

        // Web/SPA client - Authorization Code + PKCE
        if (await manager.FindByClientIdAsync("eshop-web") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "eshop-web",
                ClientType = Oidc.ClientTypes.Public,
                DisplayName = "eShop Web Client",
                RedirectUris = { new Uri("http://localhost:3000/callback") },
                PostLogoutRedirectUris = { new Uri("http://localhost:3000") },
                Permissions =
                {
                    Oidc.Permissions.Endpoints.Authorization,
                    Oidc.Permissions.Endpoints.Token,
                    Oidc.Permissions.Endpoints.Logout,
                    Oidc.Permissions.GrantTypes.AuthorizationCode,
                    Oidc.Permissions.GrantTypes.RefreshToken,
                    Oidc.Permissions.ResponseTypes.Code,
                    Oidc.Permissions.Scopes.Email,
                    Oidc.Permissions.Scopes.Profile,
                    Oidc.Permissions.Scopes.Roles,
                    Oidc.Permissions.Prefixes.Scope + "catalog-api",
                    Oidc.Permissions.Prefixes.Scope + "ordering-api",
                    Oidc.Permissions.Prefixes.Scope + "basket-api",
                }
            });
        }

        // Service-to-service client - Client Credentials
        if (await manager.FindByClientIdAsync("ordering-service") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "ordering-service",
                ClientSecret = "ordering-service-secret",
                ClientType = Oidc.ClientTypes.Confidential,
                DisplayName = "Ordering Service",
                Permissions =
                {
                    Oidc.Permissions.Endpoints.Token,
                    Oidc.Permissions.GrantTypes.ClientCredentials,
                    Oidc.Permissions.Prefixes.Scope + "catalog-api",
                }
            });
        }
    }
}
