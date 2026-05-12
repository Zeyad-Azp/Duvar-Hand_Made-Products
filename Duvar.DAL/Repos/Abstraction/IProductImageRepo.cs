using Duvar.DAL.Models;

namespace Duvar.DAL.Repos.Abstraction
{
    public interface IProductImageRepo
    {
        Task AddAsync(ProductImage productImage);
        Task DeleteAsync(int id);
        Task<List<ProductImage>> GetAllAsync();
        Task<ProductImage?> GetByIdAsync(int id);
    }
}
