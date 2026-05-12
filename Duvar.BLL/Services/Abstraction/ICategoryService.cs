using Duvar.DAL.Models;
using Duvar.BLL.ViewModels.Admin;

namespace Duvar.BLL.Services.Abstraction
{
    public interface ICategoryService
    {
        Task<List<CategoryFormViewModel>> GetAllAsync();
        Task<List<Category>> GetCategoriesWithProductsAsync();
        Task<List<Category>> GetFeaturedCategoriesAsync(int count);
        Task<int> GetTotalCategoriesCountAsync();
        Task<CategoryFormViewModel?> GetByIdAsync(int id);
        Task AddAsync(CategoryFormViewModel categoryVM);
        Task Update(CategoryFormViewModel categoryVM);
        Task DeleteAsync(int id);
    }
}
