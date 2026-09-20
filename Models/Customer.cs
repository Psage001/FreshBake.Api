using FreshBake.API.Common.Enums;

namespace FreshBake.API.Models;

public class Customer
{
    public int CustomerId { get; set; }

    public int AppliedByUserId { get; set; }
    public User AppliedByUser { get; set; } = null!;

    public string CompanyName { get; set; } = string.Empty;

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public CustomerStatus Status { get; set; } = CustomerStatus.Pending;

    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedDate { get; set; }
    public DateTime? RejectedDate { get; set; }

    public string? CustomOrderNotes { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();

    public ICollection<CustomerProduct> CustomerProducts { get; set; } = new List<CustomerProduct>();
    public ICollection<CustomerProductCategory> CustomerProductCategories { get; set; } = new List<CustomerProductCategory>();
}