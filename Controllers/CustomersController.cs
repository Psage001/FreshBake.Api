using FreshBake.API.Common.Enums;
using FreshBake.API.Data;
using FreshBake.API.Dtos.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshBake.API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Policy = "AdminOnly")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomersController ( AppDbContext context )
    {
        _context = context;
    }

    // GET /api/customers — list all customers (for Manage Customers page)
    [HttpGet]
    public async Task<ActionResult<List<CustomerDto>>> GetCustomers ()
    {
        var customers = await _context.Customers
            .Include(c => c.AppliedByUser)
            .OrderByDescending(c => c.SubmittedDate)
            .Select(c => new CustomerDto
            {
                CustomerId = c.CustomerId,
                CompanyName = c.CompanyName,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                Status = c.Status,
                SubmittedDate = c.SubmittedDate,
                ApprovedDate = c.ApprovedDate,
                RejectedDate = c.RejectedDate,
                UserName = c.AppliedByUser.Name,
                UserSurname = c.AppliedByUser.Surname,
                UserEmail = c.AppliedByUser.Email,
            })
            .ToListAsync();

        return Ok(customers);
    }

    // GET /api/customers/{id} — single customer detail
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer ( int id )
    {
        var customer = await _context.Customers
            .Include(c => c.AppliedByUser)
            .Where(c => c.CustomerId == id)
            .Select(c => new CustomerDto
            {
                CustomerId = c.CustomerId,
                CompanyName = c.CompanyName,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                Status = c.Status,
                SubmittedDate = c.SubmittedDate,
                ApprovedDate = c.ApprovedDate,
                RejectedDate = c.RejectedDate,
                UserName = c.AppliedByUser.Name,
                UserSurname = c.AppliedByUser.Surname,
                UserEmail = c.AppliedByUser.Email,
            })
            .FirstOrDefaultAsync();

        if (customer == null)
        {
            return NotFound(new { error = "Customer not found." });
        }

        return Ok(customer);
    }

    // POST /api/customers/{id}/approve
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveCustomer ( int id )
    {
        var customer = await _context.Customers
            .Include(c => c.AppliedByUser)
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (customer == null)
        {
            return NotFound(new { error = "Customer not found." });
        }

        customer.Status = CustomerStatus.Approved;
        customer.ApprovedDate = DateTime.UtcNow;

        // Approval is what actually links the applicant to the business —
        // and makes them that business's admin.
        customer.AppliedByUser.AccessLevelId = (int)AccessLevels.Customer;
        customer.AppliedByUser.CustomerId = customer.CustomerId;
        customer.AppliedByUser.IsCustomerAdmin = true;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Customer approved." });
    }

    // POST /api/customers/{id}/reject
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectCustomer ( int id )
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound(new { error = "Customer not found." });
        }

        customer.Status = CustomerStatus.Rejected;
        customer.RejectedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Customer rejected." });
    }
}