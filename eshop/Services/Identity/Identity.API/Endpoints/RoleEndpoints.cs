using Carter;
using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AppPermissions = Identity.API.Models.Permissions;

namespace Identity.API.Endpoints;

public record CreateRoleRequest(string Name, string? Description);
public record RoleDto(string Id, string Name, string? Description, IList<string> Permissions);
public record AddPermissionRequest(string Permission);

public class RoleEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/roles")
            .WithTags("Roles")
            .RequireAuthorization(AppPermissions.Users.Write);

        group.MapGet("/", GetRolesAsync)
            .WithName("GetRoles")
            .Produces<List<RoleDto>>()
            .WithSummary("Get all roles");

        group.MapPost("/", CreateRoleAsync)
            .WithName("CreateRole")
            .Produces<RoleDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create role");

        group.MapDelete("/{id}", DeleteRoleAsync)
            .WithName("DeleteRole")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete role");

        group.MapPost("/{id}/permissions", AddPermissionAsync)
            .WithName("AddPermission")
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Add permission to role");

        group.MapDelete("/{id}/permissions/{permission}", RemovePermissionAsync)
            .WithName("RemovePermission")
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Remove permission from role");
    }

    private static async Task<IResult> GetRolesAsync(
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext)
    {
        var roles = await roleManager.Roles.Include(r => r.Permissions).ToListAsync();
        var result = roles.Select(r => new RoleDto(
            r.Id, r.Name!, r.Description,
            r.Permissions.Select(p => p.Permission).ToList()
        ));
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateRoleAsync(
        CreateRoleRequest request,
        RoleManager<ApplicationRole> roleManager)
    {
        var role = new ApplicationRole { Name = request.Name, Description = request.Description };
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return Results.ValidationProblem(result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));

        return Results.Created($"/roles/{role.Id}", new RoleDto(role.Id, role.Name!, role.Description, []));
    }

    private static async Task<IResult> DeleteRoleAsync(string id, RoleManager<ApplicationRole> roleManager)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null) return Results.NotFound();

        await roleManager.DeleteAsync(role);
        return Results.NoContent();
    }

    private static async Task<IResult> AddPermissionAsync(
        string id,
        AddPermissionRequest request,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext dbContext)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null) return Results.NotFound();

        var exists = await dbContext.RolePermissions
            .AnyAsync(p => p.RoleId == id && p.Permission == request.Permission);

        if (!exists)
        {
            dbContext.RolePermissions.Add(new ApplicationRolePermission
            {
                RoleId = id,
                Permission = request.Permission
            });
            await dbContext.SaveChangesAsync();
        }

        return Results.NoContent();
    }

    private static async Task<IResult> RemovePermissionAsync(
        string id,
        string permission,
        ApplicationDbContext dbContext)
    {
        var entity = await dbContext.RolePermissions
            .FirstOrDefaultAsync(p => p.RoleId == id && p.Permission == permission);

        if (entity != null)
        {
            dbContext.RolePermissions.Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        return Results.NoContent();
    }
}
