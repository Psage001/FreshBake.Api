namespace FreshBake.API.DTOs.Product;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public int ProductCategoryId { get; set; }
}