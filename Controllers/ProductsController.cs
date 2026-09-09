using FreshBake.API.Data;
using FreshBake.API.Dtos.Product;
using FreshBake.API.DTOs.Product;
using FreshBake.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreshBake.API.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController ( AppDbContext context )
    {
        _context = context;
    }

    // GET /api/products
    [HttpGet]
    public async Task<ActionResult<List<ProductCategoryWithProductsDto>>> GetAll ()
    {
        var categories = await _context.ProductCategories
            .Include(pc => pc.Products)
            .Select(pc => new ProductCategoryWithProductsDto
            {
                ProductCategoryId = pc.ProductCategoryId,
                Name = pc.Name,
                Products = pc.Products
                    .Select(p => new ProductDto { ProductId = p.ProductId, Name = p.Name })
                    .ToList(),
            })
            .ToListAsync();

        return Ok(categories);
    }

    // POST /api/products/categories — Admin only
    [HttpPost("categories")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> CreateCategory ( [FromBody] CreateProductCategoryDto request )
    {
        var category = new ProductCategory { Name = request.Name };
        _context.ProductCategories.Add(category);
        await _context.SaveChangesAsync();
        return Ok(new { category.ProductCategoryId, category.Name });
    }

    // POST /api/products — Admin only
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> CreateProduct ( [FromBody] CreateProductDto request )
    {
        var categoryExists = await _context.ProductCategories
            .AnyAsync(pc => pc.ProductCategoryId == request.ProductCategoryId);

        if (!categoryExists)
        {
            return BadRequest(new { error = "Invalid product category." });
        }

        var product = new Product
        {
            Name = request.Name,
            ProductCategoryId = request.ProductCategoryId,
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Ok(new { product.ProductId, product.Name, product.ProductCategoryId });
    }

    // DELETE /api/products/{id} — Admin only
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> DeleteProduct ( int id )
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/products/categories/{id} — Admin only
    [HttpDelete("categories/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> DeleteCategory ( int id )
    {
        var category = await _context.ProductCategories.FindAsync(id);
        if (category == null) return NotFound();

        _context.ProductCategories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PUT /api/products/{id} — Admin only
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> UpdateProduct ( int id, [FromBody] UpdateProductDto request )
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        var categoryExists = await _context.ProductCategories
            .AnyAsync(pc => pc.ProductCategoryId == request.ProductCategoryId);

        if (!categoryExists)
        {
            return BadRequest(new { error = "Invalid product category." });
        }

        product.Name = request.Name;
        product.ProductCategoryId = request.ProductCategoryId;

        await _context.SaveChangesAsync();
        return Ok(new { product.ProductId, product.Name, product.ProductCategoryId });
    }

    // PUT /api/products/categories/{id} — Admin only
    [HttpPut("categories/{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult> UpdateCategory ( int id, [FromBody] UpdateProductCategoryDto request )
    {
        var category = await _context.ProductCategories.FindAsync(id);
        if (category == null) return NotFound();

        category.Name = request.Name;
        await _context.SaveChangesAsync();
        return Ok(new { category.ProductCategoryId, category.Name });
    }
}