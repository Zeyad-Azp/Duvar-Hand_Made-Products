using System.ComponentModel.DataAnnotations;

namespace Duvar.DAL.Models;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    // Helper: returns main image path or null
    public string? MainImagePath =>
        Images.FirstOrDefault(i => i.IsMainImage)?.ImagePath
        ?? Images.FirstOrDefault()?.ImagePath;
}
