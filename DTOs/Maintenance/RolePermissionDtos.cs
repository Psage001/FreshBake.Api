// Dtos/Maintenance/RolePermissionDtos.cs
namespace FreshBake.API.Dtos.Maintenance;

public class PagePermissionDto
{
    public int PageId { get; set; }
    public string PageName { get; set; } = string.Empty;
    public string RouteKey { get; set; } = string.Empty;
    public bool CanView { get; set; }
}

public class RolePermissionsDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<PagePermissionDto> Pages { get; set; } = new();
}

public class PagePermissionUpdateDto
{
    public int PageId { get; set; }
    public bool CanView { get; set; }
}

public class UpdateRolePermissionsRequestDto
{
    public List<PagePermissionUpdateDto> Pages { get; set; } = new();
}