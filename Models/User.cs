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
}