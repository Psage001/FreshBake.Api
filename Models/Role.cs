namespace FreshBake.API.Models;

public class Role
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Which system tier this role belongs under (Admin, Customer, SuperUser, User)
    public int AccessLevelId { get; set; }
    public AccessLevel AccessLevel { get; set; } = null!;

    public ICollection<RolePagePermission> RolePagePermissions { get; set; } = new List<RolePagePermission>();
}