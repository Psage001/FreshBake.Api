using FreshBake.API.Common.Enums;

namespace FreshBake.API.Models;

public class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpires { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int AccessLevelId { get; set; } = (int)AccessLevels.Customer;
    public AccessLevel AccessLevel { get; set; }

    // Which business this user belongs to (null until their application
    // is approved, or set directly when a customer-admin adds them as staff).
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // True for the business's own admin — lets them manage other users
    // under the same CustomerId.
    public bool IsCustomerAdmin { get; set; } = false;

    // Fine-grained page-visibility role (e.g. "Customer Manager", "Customer Employee")
    // Separate from AccessLevelId, which still governs system-level Admin/Customer/SuperUser gating.
    public int? RoleId { get; set; }
    public Role? Role { get; set; }

}