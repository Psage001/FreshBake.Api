using System.Security.Claims;
using System.Security.Cryptography;
using FreshBake.API.Common.Enums;
using FreshBake.API.Data;
using FreshBake.API.Dtos.Customer;
using FreshBake.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshBake.API.Controllers;

[ApiController]
[Route("api/customer-applications")]
[Authorize]
public class CustomerApplicationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public CustomerApplicationsController ( AppDbContext context, EmailService emailService )
    {
        _context = context;
        _emailService = emailService;
    }

    // ... your existing Submit() action stays exactly as-is ...

    // POST /api/customer-applications/staff — Admin/staff only
    [HttpPost("staff")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CustomerRegistrationResponseDto>> CreateForCustomer (
        [FromBody] CustomerManualRegistrationRequestDto request )
    {
        if (request.ExistingUserId == null && request.NewUser == null)
        {
            return BadRequest(new { error = "Provide either an existing user or new user details." });
        }

        User user;

        if (request.ExistingUserId != null)
        {
            var existingUser = await _context.Users.FindAsync(request.ExistingUserId.Value);
            if (existingUser == null)
            {
                return NotFound(new { error = "Selected user does not exist." });
            }
            user = existingUser;
        }
        else
        {
            var emailTaken = await _context.Users.AnyAsync(u => u.Email == request.NewUser!.Email);
            if (emailTaken)
            {
                return Conflict(new { error = "A user with this email already exists." });
            }

            // Random password the customer will never see — they set their own via the emailed reset link.
            var tempPassword = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));

            user = new User
            {
                Name = request.NewUser!.Name,
                Surname = request.NewUser.Surname,
                Email = request.NewUser.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword),
                ResetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
                ResetTokenExpires = DateTime.UtcNow.AddDays(7),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var resetLink = $"http://localhost:5173/reset-password?token={user.ResetToken}";
            await _emailService.SendPasswordResetEmail(user.Email, resetLink);
        }

        var existingCustomer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.UserId);

        if (existingCustomer != null)
        {
            return Conflict(new { error = "This user is already a registered customer." });
        }

        var validCategories = await _context.ProductCategories
            .Where(pc => request.ProductCategoryIds.Contains(pc.ProductCategoryId))
            .ToListAsync();

        var customer = new Customer
        {
            UserId = user.UserId,
            CompanyName = request.CompanyName,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Status = CustomerStatus.Pending,
            CustomOrderNotes = request.CustomOrderNotes,
            CustomerProductCategories = validCategories
                .Select(pc => new CustomerProductCategory { ProductCategoryId = pc.ProductCategoryId })
                .ToList(),
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return Ok(new CustomerRegistrationResponseDto
        {
            CustomerId = customer.CustomerId,
            CompanyName = customer.CompanyName,
            Latitude = customer.Latitude,
            Longitude = customer.Longitude,
            Status = customer.Status,
            SubmittedDate = customer.SubmittedDate,
            ApprovedDate = customer.ApprovedDate,
            RejectedDate = customer.RejectedDate,
            CustomOrderNotes = customer.CustomOrderNotes,
            SelectedCategories = validCategories.Select(pc => pc.Name).ToList(),
        });
    }
}