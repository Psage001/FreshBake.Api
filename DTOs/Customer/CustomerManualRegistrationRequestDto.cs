namespace FreshBake.API.Dtos.Customer;

public class CustomerManualRegistrationRequestDto
{
    // Provide ONE of these two — ExistingUserId for a known user, NewUser to create one.
    public int? ExistingUserId { get; set; }
    public NewUserDto? NewUser { get; set; }

    public string CompanyName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<int> ProductCategoryIds { get; set; } = new();
    public string? CustomOrderNotes { get; set; }
}

public class NewUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}