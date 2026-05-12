using Duvar.DAL.Data;
using Duvar.DAL.Models;

namespace Duvar.DAL.Repos.Abstraction
{
    public interface ICategoryRepo
    {
        IQueryable<Category> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category category);
        void Update(Category category);
        Task DeleteAsync(int id);
    }
}
