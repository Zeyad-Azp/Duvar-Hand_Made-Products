using Duvar.DAL.Models;

namespace Duvar.BLL.ViewModels.Admin
{
    public class AdminProductListViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public int? FilterCategoryId { get; set; }
    }

}
