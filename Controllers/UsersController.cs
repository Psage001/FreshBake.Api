using FreshBake.API.Data;
using FreshBake.API.DTOs.Users;
using FreshBake.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public UsersController ( AppDbContext context, EmailService emailService )
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers ()
    {
        var users = await _context.Users
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                AccessLevelId = u.AccessLevelId,
                AccessLevelName = u.AccessLevel.AccessLevelName
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("current-user-profile")]
    public async Task<ActionResult<UserDto>> GetCurrentUserProfile ()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            return Unauthorized();

        var dto = await _context.Users
            .Where(u => u.UserId == userId)
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                AccessLevelId = u.AccessLevelId,
                AccessLevelName = u.AccessLevel.AccessLevelName
            })
            .FirstOrDefaultAsync();

        if (dto == null) return NotFound();

        return Ok(dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser ( int id, UserDto dto )
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.Name = dto.Name;
        user.Surname = dto.Surname;
        user.Email = dto.Email;
        user.AccessLevelId = dto.AccessLevelId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser ( int id )
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Add to UsersController.cs


// POST /api/users — Admin creates a new user account
[HttpPost]
[Authorize(Policy = "AdminOnly")]
public async Task<ActionResult> CreateUser ( [FromBody] CreateUserRequestDto request )
{
    var emailTaken = await _context.Users.AnyAsync(u => u.Email == request.Email);
    if (emailTaken)
    {
        return Conflict(new { error = "A user with this email already exists." });
    }

    var tempPassword = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));

    var user = new User
    {
        Name = request.Name,
        Surname = request.Surname,
        Email = request.Email,
        AccessLevelId = request.AccessLevelId,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
        ResetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
        ResetTokenExpires = DateTime.UtcNow.AddDays(7),
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    var resetLink = $"http://localhost:5173/reset-password?token={user.ResetToken}";
    await _emailService.SendPasswordResetEmail(user.Email, resetLink);

    return Ok(new { message = "User created. Password-setup email sent." });
}
}
