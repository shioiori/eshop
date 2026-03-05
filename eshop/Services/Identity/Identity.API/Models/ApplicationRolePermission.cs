namespace Identity.API.Models;

public class ApplicationRolePermission
{
    public int Id { get; set; }
    public string RoleId { get; set; } = default!;
    public string Permission { get; set; } = default!;

    public ApplicationRole Role { get; set; } = default!;
}
