using Duvar.DAL.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Duvar.BLL.ViewModels.Admin
{
    // --- Product -----------------------------------------------------------------
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(300)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        public List<Category> Categories { get; set; } = new();

        // For create: multiple images, first becomes main
        public List<IFormFile>? NewImages { get; set; }

        // For edit: existing images
        public List<ProductImage> ExistingImages { get; set; } = new();

        // Which existing image should be set as main
        public int? MainImageIndex { get; set; }
        // Which existing images to delete (comma-separated ids)
        public string? DeleteImageIds { get; set; }
        // This will hold the FileName of the selected main image
        public string? MainImageFileName { get; set; }
    }
}
