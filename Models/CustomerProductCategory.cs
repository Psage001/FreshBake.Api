namespace FreshBake.API.Models;

public class CustomerProductCategory
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int ProductCategoryId { get; set; }
    public ProductCategory ProductCategory { get; set; } = null!;
}