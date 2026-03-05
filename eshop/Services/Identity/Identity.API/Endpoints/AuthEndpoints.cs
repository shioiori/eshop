using Carter;
using Identity.API.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.API.Endpoints;

public class AuthEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/connect/token", HandleTokenAsync)
           .WithName("Token")
           .WithSummary("Issue OAuth2 token")
           .ExcludeFromDescription(); // handled by OpenIddict passthrough

        app.MapGet("/connect/authorize", HandleAuthorizeAsync)
           .WithName("Authorize")
           .ExcludeFromDescription();

        app.MapPost("/connect/authorize", HandleAuthorizeAsync)
           .ExcludeFromDescription();

        app.MapGet("/connect/userinfo", HandleUserinfoAsync)
           .WithName("Userinfo")
           .RequireAuthorization()
           .ExcludeFromDescription();

        app.MapPost("/connect/logout", HandleLogoutAsync)
           .WithName("Logout")
           .ExcludeFromDescription();
    }

    private static async Task<IResult> HandleTokenAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext dbContext)
    {
        var request = context.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddict request cannot be retrieved.");

        if (request.IsClientCredentialsGrantType())
        {
            // Client credentials: no user principal needed, OpenIddict handles it
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            identity.AddClaim(Claims.Subject, request.ClientId!);
            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(request.GetScopes());
            return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            var result = await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var user = await userManager.FindByIdAsync(result.Principal!.GetClaim(Claims.Subject)!);
            if (user == null)
                return Results.Forbid(authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);

            var principal = await CreateUserPrincipalAsync(user, userManager, dbContext, request.GetScopes());
            return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return Results.Problem("The specified grant type is not supported.");
    }

    private static async Task<IResult> HandleAuthorizeAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext dbContext)
    {
        var request = context.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddict request cannot be retrieved.");

        var result = await context.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (!result.Succeeded)
        {
            return Results.Challenge(
                authenticationSchemes: [IdentityConstants.ApplicationScheme],
                properties: new AuthenticationProperties { RedirectUri = context.Request.PathBase + context.Request.Path + QueryString.Create(context.Request.HasFormContentType ? context.Request.Form.ToList() : context.Request.Query.ToList()) });
        }

        var user = await userManager.GetUserAsync(result.Principal)
            ?? throw new InvalidOperationException("The user cannot be retrieved.");

        var principal = await CreateUserPrincipalAsync(user, userManager, dbContext, request.GetScopes());
        return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static async Task<IResult> HandleUserinfoAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(context.User.GetClaim(Claims.Subject)!);
        if (user == null)
            return Results.Challenge(authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);

        var claims = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            [Claims.Subject] = user.Id,
            [Claims.Email] = user.Email!,
            [Claims.EmailVerified] = user.EmailConfirmed,
            [Claims.Name] = $"{user.FirstName} {user.LastName}",
            [Claims.GivenName] = user.FirstName,
            [Claims.FamilyName] = user.LastName,
        };

        return Results.Ok(claims);
    }

    private static async Task<IResult> HandleLogoutAsync(HttpContext context)
    {
        await context.SignOutAsync(IdentityConstants.ApplicationScheme);
        return Results.SignOut(authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme]);
    }

    private static async Task<ClaimsPrincipal> CreateUserPrincipalAsync(
        ApplicationUser user,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext,
        IEnumerable<string> scopes)
    {
        var identity = new ClaimsIdentity(
            authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaim(Claims.Subject, user.Id);
        identity.AddClaim(Claims.Email, user.Email!);
        identity.AddClaim(Claims.Name, $"{user.FirstName} {user.LastName}");
        identity.AddClaim(Claims.GivenName, user.FirstName);
        identity.AddClaim(Claims.FamilyName, user.LastName);

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
            identity.AddClaim(Claims.Role, role);

        // Add permission claims from roles
        var roleIds = dbContext.Roles
            .Where(r => roles.Contains(r.Name!))
            .Select(r => r.Id)
            .ToList();

        var rolePermissions = dbContext.RolePermissions
            .Where(p => roleIds.Contains(p.RoleId))
            .Select(p => p.Permission)
            .ToList();

        var userPermissions = dbContext.UserPermissions
            .Where(p => p.UserId == user.Id)
            .Select(p => p.Permission)
            .ToList();

        var allPermissions = rolePermissions.Union(userPermissions).ToList();

        if (allPermissions.Count > 0)
            identity.AddClaim("permissions", string.Join(",", allPermissions));

        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(scopes);
        principal.SetDestinations(GetDestinations);

        return principal;
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        return claim.Type switch
        {
            Claims.Name or Claims.Email =>
                [Destinations.AccessToken, Destinations.IdentityToken],
            Claims.Role or "permissions" =>
                [Destinations.AccessToken, Destinations.IdentityToken],
            _ => [Destinations.AccessToken]
        };
    }
}
