namespace Identity.API.Models;

public class ApplicationUserPermission
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public string Permission { get; set; } = default!;

    public ApplicationUser User { get; set; } = default!;
}
