using FreshBake.API.Data;
using FreshBake.API.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<UserListDto>>> GetUsers ()
    {
        var users = await _context.Users
            .Select(u => new UserListDto
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser ( int id, UserListDto dto )
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