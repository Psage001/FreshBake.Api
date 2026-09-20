// Models/Page.cs
namespace FreshBake.API.Models;

public class Page
{
    public int PageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RouteKey { get; set; } = string.Empty;

    public ICollection<RolePagePermission> RolePagePermissions { get; set; } = new List<RolePagePermission>();
}