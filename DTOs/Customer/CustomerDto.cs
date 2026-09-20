using FreshBake.API.Common.Enums;

namespace FreshBake.API.Dtos.Customer;

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public CustomerStatus Status { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? CustomOrderNotes { get; set; }

    public string UserName { get; set; } = string.Empty;
    public string UserSurname { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
}