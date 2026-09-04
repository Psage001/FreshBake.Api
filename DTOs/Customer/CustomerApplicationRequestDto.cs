
namespace FreshBake.API.Dtos.Customer;

public class CustomerApplicationRequestDto
{
    public string CompanyName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}