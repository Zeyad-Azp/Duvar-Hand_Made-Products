using Duvar.DAL.Models;

namespace Duvar.DAL.Repos.Abstraction
{
    public interface IProductRepo
    {
        IQueryable<Product> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        void Update(Product product);
        Task DeleteAsync(int id);
    }
}
