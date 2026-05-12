using Duvar.DAL.Models;
using Duvar.BLL.ViewModels;
using Duvar.BLL.ViewModels.Admin;
using Microsoft.AspNetCore.Http;
namespace Duvar.BLL.Services.Abstraction
{
    public interface IProductService
    {
        IQueryable<ProductFormViewModel> GetAllAsync(int? categoryId);
        Task<ProductFormViewModel?> GetByIdAsync(int id);
        Task AddAsync(ProductFormViewModel productVM);
        Task Update(int id , ProductFormViewModel productVM);
        Task DeleteAsync(int id);
        void HandleImageDeletions(Product product, string deleteImageIds);
        Task HandleNewImages(Product product, List<IFormFile> newFiles, string? mainImageFileName = null);
        Task<AdminProductListViewModel> GetAdminProductListAsync(int? categoryId);
        Task SetMainImageAsync(int imageId, int productId);
        Task<ProductListViewModel> GetProductListAsync(int? categoryId, string? search);
        Task<List<Product>> GetFilteredProductsAsync(int? categoryId, string? search);
        Task<ProductDetailsViewModel?> GetProductDetailsAsync(int id);
        Task<List<Product>> GetLatestProductsAsync(int count);
        Task<int> GetTotalProductsCountAsync();
    }
}
