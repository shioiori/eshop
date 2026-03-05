using Carter;
using Identity.API.Data;
using Identity.API.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppRoles = Identity.API.Models.Roles;
using AppPermissions = Identity.API.Models.Permissions;

namespace Identity.API.Endpoints;

public record RegisterUserRequest(string Email, string Password, string FirstName, string LastName);
public record RegisterUserResponse(string Id, string Email);
public record UpdateUserRequest(string FirstName, string LastName, bool IsActive);
public record UserDto(string Id, string Email, string FirstName, string LastName, bool IsActive, IList<string> Roles);
public record AddUserPermissionRequest(string Permission);

public class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users");

        group.MapPost("/register", RegisterAsync)
            .WithName("RegisterUser")
            .Produces<RegisterUserResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Register a new user")
            .WithDescription("Register a new user");

        group.MapGet("/", GetUsersAsync)
            .WithName("GetUsers")
            .Produces<List<UserDto>>()
            .RequireAuthorization(AppPermissions.Users.Write)
            .WithSummary("Get all users");

        group.MapGet("/{id}", GetUserByIdAsync)
            .WithName("GetUserById")
            .Produces<UserDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .WithSummary("Get user by ID");

        group.MapPut("/{id}", UpdateUserAsync)
            .WithName("UpdateUser")
            .Produces<UserDto>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(AppPermissions.Users.Write)
            .WithSummary("Update user");

        group.MapDelete("/{id}", DeleteUserAsync)
            .WithName("DeleteUser")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(AppPermissions.Users.Write)
            .WithSummary("Delete user");

        group.MapPost("/{id}/roles/{roleName}", AssignRoleAsync)
            .WithName("AssignRole")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(AppPermissions.Users.Write)
            .WithSummary("Assign role to user");

        group.MapDelete("/{id}/roles/{roleName}", RemoveRoleAsync)
            .WithName("RemoveRole")
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(AppPermissions.Users.Write)
            .WithSummary("Remove role from user");

        group.MapGet("/{id}/permissions", GetUserPermissionsAsync)
            .WithName("GetUserPermissions")
            .Produces<List<string>>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(AppPermissions.Users.Write)
            .WithSummary("Get permissions assigned directly to user");

        group.MapPost("/{id}/permissions", AddUserPermissionAsync)
            .WithName("AddUserPermission")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization(AppPermissions.Users.Write)
            .WithSummary("Add permission directly to user");

        group.MapDelete("/{id}/permissions/{permission}", RemoveUserPermissionAsync)
            .WithName("RemoveUserPermission")
            .Produces(StatusCodes.Status204NoContent)
            .RequireAuthorization(AppPermissions.Users.Write)
            .WithSummary("Remove permission directly from user");
    }

    private static async Task<IResult> RegisterAsync(
        RegisterUserRequest request,
        UserManager<ApplicationUser> userManager)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));

        await userManager.AddToRoleAsync(user, AppRoles.Customer);

        return Results.Created($"/users/{user.Id}", new RegisterUserResponse(user.Id, user.Email!));
    }

    private static async Task<IResult> GetUsersAsync(UserManager<ApplicationUser> userManager)
    {
        var users = await userManager.Users.ToListAsync();
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add(new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, user.IsActive, roles));
        }
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserByIdAsync(string id, UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return Results.NotFound();

        var roles = await userManager.GetRolesAsync(user);
        return Results.Ok(new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, user.IsActive, roles));
    }

    private static async Task<IResult> UpdateUserAsync(
        string id,
        UpdateUserRequest request,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return Results.NotFound();

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.IsActive = request.IsActive;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));

        var roles = await userManager.GetRolesAsync(user);
        return Results.Ok(new UserDto(user.Id, user.Email!, user.FirstName, user.LastName, user.IsActive, roles));
    }

    private static async Task<IResult> DeleteUserAsync(string id, UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return Results.NotFound();

        await userManager.DeleteAsync(user);
        return Results.NoContent();
    }

    private static async Task<IResult> AssignRoleAsync(
        string id,
        string roleName,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return Results.NotFound();

        var result = await userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));

        return Results.NoContent();
    }

    private static async Task<IResult> RemoveRoleAsync(
        string id,
        string roleName,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return Results.NotFound();

        await userManager.RemoveFromRoleAsync(user, roleName);
        return Results.NoContent();
    }

    private static async Task<IResult> GetUserPermissionsAsync(
        string id,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return Results.NotFound();

        var permissions = await dbContext.UserPermissions
            .Where(p => p.UserId == id)
            .Select(p => p.Permission)
            .ToListAsync();

        return Results.Ok(permissions);
    }

    private static async Task<IResult> AddUserPermissionAsync(
        string id,
        AddUserPermissionRequest request,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return Results.NotFound();

        var exists = await dbContext.UserPermissions
            .AnyAsync(p => p.UserId == id && p.Permission == request.Permission);

        if (!exists)
        {
            dbContext.UserPermissions.Add(new ApplicationUserPermission
            {
                UserId = id,
                Permission = request.Permission
            });
            await dbContext.SaveChangesAsync();
        }

        return Results.NoContent();
    }

    private static async Task<IResult> RemoveUserPermissionAsync(
        string id,
        string permission,
        ApplicationDbContext dbContext)
    {
        var entity = await dbContext.UserPermissions
            .FirstOrDefaultAsync(p => p.UserId == id && p.Permission == permission);

        if (entity != null)
        {
            dbContext.UserPermissions.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        return Results.NoContent();
    }
}
