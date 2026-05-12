using Duvar.DAL.Models;

namespace Duvar.BLL.ViewModels.Admin
{
    // --- Dashboard ---------------------------------------------------------------
    public class DashboardViewModel
    {
        public int TotalCategories { get; set; }
        public int TotalProducts { get; set; }
        public List<Product> LatestProducts { get; set; } = new();
    }
}
