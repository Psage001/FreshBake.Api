namespace FreshBake.API.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;

    public int ProductCategoryId { get; set; }
    public ProductCategory ProductCategory { get; set; } = null!;
}