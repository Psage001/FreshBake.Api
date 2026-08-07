// Controllers/AuthController.cs
using FreshBake.API.Data;
using FreshBake.API.DTOs;
using FreshBake.API.DTOs.Auth;
using FreshBake.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace FreshBake.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    private readonly IConfiguration _config;

    public AuthController ( AppDbContext context, IConfiguration config )
    {
        _context = context;
        _config = config;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register ( RegisterDto dto )
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { error = "Email and password are required." });
        }

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existingUser != null)
        {
            return Conflict(new { error = "A user with this email already exists." });
        }

        var user = new User
        {
            Name = dto.Name,
            Surname = dto.Surname,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return StatusCode(201, new { message = "User registered successfully." });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login ( LoginDto dto )
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new { error = "Invalid email or password." });
        }

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            token,
            user = new
            {
                user.UserId,
                user.Name,
                user.Surname,
                user.Email
            }
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword ( ForgotPasswordDto dto )
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        // Always respond the same way whether or not the user exists —
        // otherwise this endpoint leaks which emails are registered.
        if (user == null)
        {
            return Ok(new { message = "If that email exists, a reset link has been sent." });
        }

        var resetToken = GenerateSecureToken();
        user.ResetToken = resetToken;
        user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(30);

        await _context.SaveChangesAsync();

        var resetLink = $"http://localhost:5173/reset-password?token={resetToken}";

        // TODO: send this via real email (e.g. SendGrid, SMTP) instead of logging it
        Console.WriteLine($"Password reset link: {resetLink}");

        return Ok(new { message = "If that email exists, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword ( ResetPasswordDto dto )
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.ResetToken == dto.Token && u.ResetTokenExpires > DateTime.UtcNow);

        if (user == null)
        {
            return BadRequest(new { error = "Invalid or expired reset token." });
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.ResetToken = null;
        user.ResetTokenExpires = null;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Password updated successfully. You can now log in." });
    }

    private static string GenerateSecureToken ()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToHexString(bytes); // URL-safe, no special characters
    }

    private string GenerateJwtToken ( User user )
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}")
    };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"]!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    //test github connection on api code
}