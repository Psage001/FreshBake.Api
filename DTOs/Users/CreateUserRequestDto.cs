namespace FreshBake.API.DTOs.Users;

public class CreateUserRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int AccessLevelId { get; set; }
}