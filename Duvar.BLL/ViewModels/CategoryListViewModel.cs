using Duvar.DAL.Models;

namespace Duvar.BLL.ViewModels
{
    // --- Categories -------------------------------------------------------------
    public class CategoryListViewModel
    {
        public List<Category> Categories { get; set; } = new();
    }
}
