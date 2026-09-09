namespace FreshBake.API.Dtos.Product;

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public int ProductCategoryId { get; set; }
}
