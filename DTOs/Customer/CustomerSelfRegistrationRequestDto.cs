namespace FreshBake.API.Dtos.Customer;

public class CustomerSelfRegistrationRequestDto
{
    public string CompanyName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<int> ProductCategoryIds { get; set; } = new();
    public string? CustomOrderNotes { get; set; }
}