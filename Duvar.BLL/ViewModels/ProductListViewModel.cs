using Duvar.DAL.Models;

namespace Duvar.BLL.ViewModels
{
    // --- Products ---------------------------------------------------------------
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public int? SelectedCategoryId { get; set; }
        public string? SelectedCategoryName { get; set; }
        public string? SearchTerm { get; set; }
    }
}
