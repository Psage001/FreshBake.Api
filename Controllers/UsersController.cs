using FreshBake.API.Data;
using FreshBake.API.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController ( AppDbContext context )
    {
        _context = context;
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
}
