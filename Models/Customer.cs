using FreshBake.API.Common.Enums;

namespace FreshBake.API.Models;

public class Customer
{
    public int CustomerId { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string CompanyName { get; set; } = string.Empty;

    // Business location — deliberately separate from the User's own
    // address/location, since the user's personal location and the
    // business location can differ (e.g. lives in Cape Town, business in George).
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public CustomerStatus Status { get; set; } = CustomerStatus.Pending;

    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedDate { get; set; }
    public DateTime? RejectedDate { get; set; }
}