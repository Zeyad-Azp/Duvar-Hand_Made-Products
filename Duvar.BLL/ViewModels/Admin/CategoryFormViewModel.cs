using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Duvar.BLL.ViewModels.Admin
{
    // --- Category ----------------------------------------------------------------
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? CurrentImagePath { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
