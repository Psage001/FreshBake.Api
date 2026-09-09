using FreshBake.API.DTOs.Product;

namespace FreshBake.API.DTOs.Product;

public class ProductCategoryWithProductsDto
{
    public int ProductCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ProductDto> Products { get; set; } = new();
}