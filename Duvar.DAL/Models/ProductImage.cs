using System.ComponentModel.DataAnnotations;

namespace Duvar.DAL.Models;

public class ProductImage
{
    public int Id { get; set; }

    [Required]
    public string ImagePath { get; set; } = string.Empty;

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public bool IsMainImage { get; set; } = false;
}
