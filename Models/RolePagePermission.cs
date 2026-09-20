// Models/RolePagePermission.cs
namespace FreshBake.API.Models;

public class RolePagePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int PageId { get; set; }
    public Page Page { get; set; } = null!;

    public bool CanView { get; set; }
}