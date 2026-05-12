using Duvar.DAL.Models;

namespace Duvar.BLL.Services.Abstraction
{
    public interface IProductImageService
    {
        Task<List<ProductImage>> GetAllAsync();
        Task AddAsync(ProductImage productImage);
        Task DeleteAsync(int id);
        Task<ProductImage?> GetByIdAsync(int id);
    }
}
