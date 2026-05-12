using Duvar.DAL.Models;

namespace Duvar.BLL.ViewModels
{
    // --- Home -------------------------------------------------------------------
    public class HomeViewModel
    {
        public List<Category> FeaturedCategories { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
    }
}
